using System;
using System.Text;
using FluentAssertions;
using FluentCsv.CsvParser.Splitters;
using FluentCsv.Exceptions;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    public class Rfc4180DataSplitterShould
    {
        [Test]
        [TestCase(";", "Aurelien;BOUDOUX;\"9\r\nrue du test; impasse\r\n75001\r\nParis\"", "Aurelien", "BOUDOUX","9\r\nrue du test; impasse\r\n75001\r\nParis")]
        [TestCase("<->", "Aurelien<->BOUDOUX<->\"9\r\nrue du test; <-> impasse <->\r\n75001\r\nParis\"", "Aurelien","BOUDOUX", "9\r\nrue du test; <-> impasse <->\r\n75001\r\nParis")]
        [TestCase(",", "\"M. Aurelien BOUDOUX, TEST\",,", "M. Aurelien BOUDOUX, TEST", "", "")]
        [TestCase(",", "\"TEST1\",\"TEST2\",\"TEST3\"", "TEST1", "TEST2", "TEST3")]
        [TestCase("|", "\"TEST1\"|\"TEST2\"|\"TEST3\"", "TEST1", "TEST2", "TEST3")]
        [TestCase(",", "TEST1,TEST2,TEST3", "TEST1", "TEST2", "TEST3")]
        [TestCase(",", "TEST1,\"TEST2,TEST3\",TEST4", "TEST1", "TEST2,TEST3", "TEST4")]
        [TestCase(",", "TEST1,\"TEST2,TEST3\",", "TEST1", "TEST2,TEST3", "")]
        [TestCase(",", "\"Aurelien, \"\"BOUDOUX\"\"\",TEST,", "Aurelien, \"BOUDOUX\"", "TEST", "")]
        [TestCase(",", "\"\"\"\",\"\",\" \"\" \"", "\"", "", " \" ")]
        public void SplitColumnsInRfc4180(string delimiter, string input, string expected1, string expected2, string expected3)
        {
            var splitter = new Rfc4180DataSplitter();
            var result = splitter.SplitColumns(input, delimiter);

            result.Should().HaveCount(3);
            result[0].Should().Be(expected1);
            result[1].Should().Be(expected2);
            result[2].Should().Be(expected3);
        }

        [Test]        
        [TestCase("\r\n", "Aurelien;BOUDOUX;\"9\r\nrue du test; impasse\r\n75001\r\nParis\"\r\n\"bonjour\"\r\ntest", "Aurelien;BOUDOUX;\"9\r\nrue du test; impasse\r\n75001\r\nParis\"", "\"bonjour\"", "test")]
        [TestCase("\r\n", "Firstname;Address\r\nTEST1;\"10\r\nrue du test\"\r\n\"TEST2\r\nfirst\r\n\";OK", "Firstname;Address", "TEST1;\"10\r\nrue du test\"", "\"TEST2\r\nfirst\r\n\";OK")]
        [TestCase("<endl>", "Column1;\"Column2\"<endl>\"test<endl>;test\"<endl>hello;ok", "Column1;\"Column2\"", "\"test<endl>;test\"", "hello;ok")]
        public void ReplaceAllNewLIneWithinDoubleQuotes(string delimiter, string input, string expected1, string expected2, string expected3)
        {
            var splitter = new Rfc4180DataSplitter();
            var result = splitter.SplitLines(input, delimiter);

            result.Should().HaveCount(3);
            result[0].Should().Be(expected1);
            result[1].Should().Be(expected2);
            result[2].Should().Be(expected3);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void DontThrowErrorIfSplitEmptyLine(string input)
        {
            var splitter = new Rfc4180DataSplitter();
            var result = splitter.SplitLines(input, "\r\n");

            result.Should().HaveCount(1);
            result[0].Should().BeEmpty();
        }

        [Test]
        [TestCase("\r\n","Test\r\n\"coucou")]
        public void ThrowErrorIfNoEndQuoteFoundWhenExtractingLines(string delimiter, string input)
        {
            var splitter = new Rfc4180DataSplitter();
            Action action = () => splitter.SplitLines(input, delimiter);
            action.Should().Throw<MissingQuoteException>();
        }

        [Test]
        [TestCase("\r\n","Column1;Column2","Column1;Column2")]
        [TestCase("\r\n","Column1;Column2\r\ntest;test", "Column1;Column2")]
        [TestCase("<endl>","Column1;Column2<endl>test;test", "Column1;Column2")]
        [TestCase("\r\n","\"C1\r\n1\";\"C2\r\n2\"\r\nA;B","\"C1\r\n1\";\"C2\r\n2\"")]
        [TestCase("\r\n", "", "")]
        public void ExtractOnlyFirstLine(string delimiter, string input, string expected)
        {
            var splitter = new Rfc4180DataSplitter();
            var firstLine = splitter.GetFirstLine(input, delimiter);
            firstLine.Should().Be(expected);
        }

        [Test]
        public void ThrowErrorIfUsingDoubleQuoteAsLineDelimiter()
        {
            const string input = "Header1;Header2\"Value1;Value2\"";

            var splitter = new Rfc4180DataSplitter();
            Action action = () => splitter.SplitLines(input, "\"");

            action.Should().Throw<BadDelimiterException>();
        }

        [Test]
        public void ThrowErrorIfUsingDoubleQuoteAsColumnsDelimiter()
        {
            const string input = "Header1\"Header2\"Value1\"Value2\"";

            var splitter = new Rfc4180DataSplitter();
            Action action = () => splitter.SplitColumns(input, "\"");

            action.Should().Throw<BadDelimiterException>();
        }

        [Test]
        public void ThrowErrorIfUsingDoubleQuoteAsGetFirstLineDelimiter()
        {
            const string input = "Header1\"Header2\"Value1\"Value2\"";

            var splitter = new Rfc4180DataSplitter();
            Action action = () => splitter.GetFirstLine(input, "\"");

            action.Should().Throw<BadDelimiterException>();
        }

		[Test]
	    public void RemoveIfStringContainsBom()
		{
			const string input = "\uFEFF\"TEST1\",\"TEST2\",\"TEST3\"\r\nA,B,C";

			var splitter = new Rfc4180DataSplitter();
			var firstLine = splitter.GetFirstLine(input, "\r\n");
			var columns = splitter.SplitColumns(firstLine,",");

			columns.Should().HaveCount(3);
			columns[0].Should().Be("TEST1");
		}
    }
}