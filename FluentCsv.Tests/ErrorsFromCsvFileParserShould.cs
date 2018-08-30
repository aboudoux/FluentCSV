﻿using System;
using System.Globalization;
using System.Text;
using FluentAssertions;
using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Splitters;
using FluentCsv.Tests.Results;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    public class ErrorsFromCsvFileParserShould
    {
        private CsvFileParser<TestResult> GetParser(string input)
            => new CsvFileParser<TestResult>(input, new SimpleDataSplitter(), new CultureInfo("fr-FR"));

        [Test]
        public void ReturnLineAndMessageOfBadDateConversion()
        {
            const string input = "01/01/2001\r\n01/01/2002\r\n51/01/2003\r\n01/01/2004";

            var parser = GetParser(input);
            parser.AddColumn(0, a=>a.Member3);
            var result = parser.Parse();

            result.Errors.ShouldContainEquivalentTo(new CsvParseError(3,0,null, "La chaîne n'a pas été reconnue en tant que DateTime valide."));
        }

        [Test]
        public void ProvideCorrectFileNumberIfFirstLineDeclaredAsHeader()
        {
            const string input = "Header\r\ntest1\r\ntest2\r\nbad\r\ntest3";

            var parser = GetParser(input);
            parser.DeclareFirstLineHasHeader();
            parser.AddColumn(0, a => a.Member1, CheckBadString);

            var result = parser.Parse();
            result.Errors.ShouldContainEquivalentTo(new CsvParseError(4, 0, "Header", "bad string found"));

            string CheckBadString(string source)
            {
                if (source == "bad")
                    throw new Exception("bad string found");
                return source;
            }
        }

		[Test]
	    public void WriteErrorInFile()
	    {
			var fakeFile = new FakeFileWriter();
			var result = new ParseCsvResult<TestResult>(
				Array.Empty<TestResult>(), 
				new[]
				{
					new CsvParseError(0,0,"TEST0", "this is a test 0"),
					new CsvParseError(1,1,"TEST1", "this is \"a\" test 1"),
					new CsvParseError(2,2,"TEST2", "this is a test 2"),
				}, fakeFile);

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