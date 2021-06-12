using System;
using System.Globalization;
using System.Text;
using FluentAssertions;
using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Results;
using FluentCsv.CsvParser.Splitters;
using FluentCsv.Tests.Results;
using Xunit;

namespace FluentCsv.Tests
{
    public class ErrorsFromCsvFileParserShould
    {
        private CsvFileParser<TestResult> GetParser(string input)
            => new CsvFileParser<TestResult>(input, new SimpleDataSplitter(), new CultureInfo("fr-FR"));

        [Fact]
        public void ReturnLineAndMessageOfBadDateConversion()
        {
            const string input = "01/01/2001\r\n01/01/2002\r\n51/01/2003\r\n01/01/2004";

            var parser = GetParser(input);
            parser.AddColumn(0, a=>a.Member3);
            var result = parser.Parse(new ArrayCsvResult<TestResult>());

            result.Errors.ShouldContainEquivalentTo(new CsvParseError(3,0,null, "String '51/01/2003' was not recognized as a valid DateTime."));
        }

        [Fact]
        public void ProvideCorrectFileNumberIfFirstLineDeclaredAsHeader()
        {
            const string input = "Header\r\ntest1\r\ntest2\r\nbad\r\ntest3";

            var parser = GetParser(input);
            parser.DeclareFirstLineHasHeader();
            parser.AddColumn(0, a => a.Member1, CheckBadString);

            var result = parser.Parse(new ArrayCsvResult<TestResult>());
            result.Errors.ShouldContainEquivalentTo(new CsvParseError(4, 0, "Header", "bad string found"));

            string CheckBadString(string source)
            {
                if (source == "bad")
                    throw new Exception("bad string found");
                return source;
            }
        }

		[Fact]
	    public void WriteErrorInFile()
	    {
			var fakeFile = new FakeFileWriter();
			var result = new ArrayCsvResult<TestResult>(fakeFile);
			result.Fill(Array.Empty<TestResult>(),
				new[]
				{
					new CsvParseError(0,0,"TEST0", "this is a test 0"),
					new CsvParseError(1,1,"TEST1", "this is \"a\" test 1"),
					new CsvParseError(2,2,"TEST2", "this is a test 2"),
				});

		    const string expectedoutput = @"Line;ColumnZeroBaseIndex;ColumnName;Message
0;0;""TEST0"";""this is a test 0""
1;1;""TEST1"";""this is """"a"""" test 1""
2;2;""TEST2"";""this is a test 2""
";

			result.SaveErrorsInFile("test.csv");
		    fakeFile.Data.Should().Be(expectedoutput);
	    }
    }

	public class FakeFileWriter : IFileWriter
	{
		public string FilePath { get; private set; }
		public string Data { get; private set; }
		public Encoding Encoding { get; private set; }

		public void Write(string filePath, string data, Encoding encoding)
		{
			FilePath = filePath;
			Data = data;
			Encoding = encoding;
		}
	}
}