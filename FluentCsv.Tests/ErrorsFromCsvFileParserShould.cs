using System;
using FluentCsv.CsvParser;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    public class ErrorsFromCsvFileParserShould
    {
        [Test]
        public void ReturnLineAndMessageOfBadDateConversion()
        {
            const string input = "01/01/2001\r\n01/01/2002\r\n51/01/2003\r\n01/01/2004";

            var parser = new CsvFileParser<TestResult>(input);
            parser.AddColumn(0, a=>a.Member3);
            var result = parser.Parse();

            result.Errors.ShouldContainEquivalentTo(new CsvParseError(3,0,"", "La chaîne n'a pas été reconnue en tant que DateTime valide."));
        }

        [Test]
        public void ProvideCorrectFileNumberIfFirstLineDeclaredAsHeader()
        {
            const string input = "Header\r\ntest1\r\ntest2\r\nbad\r\ntest3";

            var parser = new CsvFileParser<TestResult>(input);
            parser.DeclareFirstLineHasHeader();
            parser.AddColumn(0, a => a.Member1, CheckBadString);

            var result = parser.Parse();
            result.Errors.ShouldContainEquivalentTo(new CsvParseError(4, 0, "", "bad string found"));

            string CheckBadString(string source)
            {
                if (source == "bad")
                    throw new Exception("bad string found");
                return source;
            }
        }
    }
}