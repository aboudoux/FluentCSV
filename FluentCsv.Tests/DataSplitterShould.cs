using System;
using FluentAssertions;
using FluentCsv.CsvParser.Splitters;
using FluentCsv.Exceptions;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    public class DataSplitterShould
    {
        [Test]
        [TestCase(";", "Aurelien;BOUDOUX;\"9\r\nrue du test; impasse\r\n75001\r\nParis\"", "Aurelien", "BOUDOUX","9\r\nrue du test; impasse\r\n75001\r\nParis")]
        [TestCase("<->", "Aurelien<->BOUDOUX<->\"9\r\nrue du test; <-> impasse <->\r\n75001\r\nParis\"", "Aurelien","BOUDOUX", "9\r\nrue du test; <-> impasse <->\r\n75001\r\nParis")]
        [TestCase(",", "\"M. Aurelien BOUDOUX, TEST\",,", "M. Aurelien BOUDOUX, TEST", "", "")]
        [TestCase(",", "\"TEST1\",\"TEST2\",\"TEST3\"", "TEST1", "TEST2", "TEST3")]
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
    }
}