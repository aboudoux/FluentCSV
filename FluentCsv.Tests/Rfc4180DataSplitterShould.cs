using System;
using FluentAssertions;
using FluentCsv.CsvParser.Splitters;
using FluentCsv.Exceptions;
using Xunit;

namespace FluentCsv.Tests
{
    public class Rfc4180DataSplitterShould
    {
        [Theory]
        [InlineData(";", "Aurelien;BOUDOUX;\"9\r\nrue du test; impasse\r\n75001\r\nParis\"", "Aurelien", "BOUDOUX","9\r\nrue du test; impasse\r\n75001\r\nParis")]
        [InlineData("<->", "Aurelien<->BOUDOUX<->\"9\r\nrue du test; <-> impasse <->\r\n75001\r\nParis\"", "Aurelien","BOUDOUX", "9\r\nrue du test; <-> impasse <->\r\n75001\r\nParis")]
        [InlineData(",", "\"M. Aurelien BOUDOUX, TEST\",,", "M. Aurelien BOUDOUX, TEST", "", "")]
        [InlineData(",", "\"TEST1\",\"TEST2\",\"TEST3\"", "TEST1", "TEST2", "TEST3")]
        [InlineData("|", "\"TEST1\"|\"TEST2\"|\"TEST3\"", "TEST1", "TEST2", "TEST3")]
        [InlineData("^", "\"TEST1\"^\"TEST2\"^\"TEST3\"", "TEST1", "TEST2", "TEST3")]
        [InlineData("$", "\"TEST1\"$\"TEST2\"$\"TEST3\"", "TEST1", "TEST2", "TEST3")]
        [InlineData(",", "TEST1,TEST2,TEST3", "TEST1", "TEST2", "TEST3")]
        [InlineData(",", "TEST1,\"TEST2,TEST3\",TEST4", "TEST1", "TEST2,TEST3", "TEST4")]
        [InlineData(",", "TEST1,\"TEST2,TEST3\",", "TEST1", "TEST2,TEST3", "")]
        [InlineData(",", "\"Aurelien, \"\"BOUDOUX\"\"\",TEST,", "Aurelien, \"BOUDOUX\"", "TEST", "")]
        [InlineData(",", "\"\"\"ok\",\"\",\" \"\" \"", "\"ok", "", " \" ")]
        [InlineData(",", "\"\"\"ok\"\"\",\"\",\" \"\" \"", "\"ok\"", "", " \" ")]        
        public void SplitColumnsInRfc4180(string delimiter, string input, string expected1, string expected2, string expected3)
        {
            var splitter = new Rfc4180DataSplitter();
            var result = splitter.SplitColumns(input, delimiter);

            result.Should().HaveCount(3);
            result[0].Should().Be(expected1);
            result[1].Should().Be(expected2);
            result[2].Should().Be(expected3);
        }

        [Theory]        
        [InlineData("\r\n", "Aurelien;BOUDOUX;\"9\r\nrue du test; impasse\r\n75001\r\nParis\"\r\n\"bonjour\"\r\ntest", "Aurelien;BOUDOUX;\"9\r\nrue du test; impasse\r\n75001\r\nParis\"", "\"bonjour\"", "test")]
        [InlineData("\r\n", "Firstname;Address\r\nTEST1;\"10\r\nrue du test\"\r\n\"TEST2\r\nfirst\r\n\";OK", "Firstname;Address", "TEST1;\"10\r\nrue du test\"", "\"TEST2\r\nfirst\r\n\";OK")]
        [InlineData("<endl>", "Column1;\"Column2\"<endl>\"test<endl>;test\"<endl>hello;ok", "Column1;\"Column2\"", "\"test<endl>;test\"", "hello;ok")]
        public void ReplaceAllNewLIneWithinDoubleQuotes(string delimiter, string input, string expected1, string expected2, string expected3)
        {
            var splitter = new Rfc4180DataSplitter();
            var result = splitter.SplitLines(input, delimiter);

            result.Should().HaveCount(3);
            result[0].Should().Be(expected1);
            result[1].Should().Be(expected2);
            result[2].Should().Be(expected3);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void DontThrowErrorIfSplitEmptyLine(string input)
        {
            var splitter = new Rfc4180DataSplitter();
            var result = splitter.SplitLines(input, "\r\n");

            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData("\r\n","Test\r\n\"coucou\r\n")]
        public void ThrowErrorIfNoEndQuoteFoundWhenExtractingLines(string delimiter, string input)
        {
            var splitter = new Rfc4180DataSplitter();
            Action action = () => splitter.SplitLines(input, delimiter);
            action.Should().Throw<MissingQuoteException>();
        }

        [Theory]
        [InlineData("\r\n","Column1;Column2","Column1;Column2")]
        [InlineData("\r\n","Column1;Column2\r\ntest;test", "Column1;Column2")]
        [InlineData("<endl>","Column1;Column2<endl>test;test", "Column1;Column2")]
        [InlineData("\r\n","\"C1\r\n1\";\"C2\r\n2\"\r\nA;B","\"C1\r\n1\";\"C2\r\n2\"")]
        [InlineData("\r\n", "", "")]
        public void ExtractOnlyFirstLine(string delimiter, string input, string expected)
        {
            var splitter = new Rfc4180DataSplitter();
            var firstLine = splitter.GetFirstLine(input, delimiter);
            firstLine.Should().Be(expected);
        }

        [Fact]
        public void ThrowErrorIfUsingDoubleQuoteAsLineDelimiter()
        {
            var splitter = new Rfc4180DataSplitter();
            Action action = () => splitter.EnsureDelimitersAreValid("\"", ",");
            action.Should().Throw<BadDelimiterException>();
        }

        [Fact]
        public void ThrowErrorIfUsingDoubleQuoteAsColumnsDelimiter()
        {
            var splitter = new Rfc4180DataSplitter();
            Action action = () => splitter.EnsureDelimitersAreValid(",", "\"");
            action.Should().Throw<BadDelimiterException>();
        }

		[Fact]
	    public void TestReplace()
		{
			const string doubleQuote = "\"\"";
			const string simpleQuote = "\"";

			var data = doubleQuote + doubleQuote;
			var result = data.Replace(doubleQuote, simpleQuote);
			result.Should().Be(doubleQuote);
		}

		[Fact]
	    public void DoubleQuoteTest()
	    {
		    const string input = @"277;237C3989-4D81-4F62-9581-A19516D08D86;2018-07-16 09:56:08.780;37;""{""""AggregateId"""":""""237c3989-4d81-4f62-9581-a19516d08d86"""",""""SiteId"""":""""237c3989-4d81-4f62-9581-a19516d08d86"""",""""Contact"""":{""""ContactId"""":""""42497b38-f0e4-497c-918f-61fb27d5a208"""",""""Nom"""":""""BRUCKER"""",""""Prenom"""":"""""""",""""TypeContactSiteId"""":""""7bfa472a-ab24-44d8-917b-360d1204364b"""",""""MoyensDeContact"""":{""""TelephoneFixe"""":"""""""",""""TelephonePortable"""":""""+33603644655"""",""""Email"""":"""""""",""""AdressePostale"""":{""""NumeroEtVoie"""":""""10 Rue du Lac"""",""""Complement"""":"""""""",""""CodePostal"""":""""67114"""",""""Commune"""":""""Eschau"""",""""Pays"""":""""France""""}},""""AutresInformations"""":""""""""},""""HorodateCreationUtc"""":""""2018-07-16T09:56:08.6962731Z"""",""""Journalisation"""":{""""UtilisateurId"""":""""00000000-0000-0000-0000-000000000000"""",""""TacheId"""":-1,""""Horodate"""":""""0001-01-01T00:00:00""""}}""";

		    var splitter = new Rfc4180DataSplitter();

		    var columns =splitter.SplitColumns(input, ";");
		    columns[4].Should().Contain("\"Complement\":\"\"");
	    }
	}
}