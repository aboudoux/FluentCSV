using System;
using System.Globalization;
using FluentAssertions;
using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Results;
using FluentCsv.CsvParser.Splitters;
using FluentCsv.Exceptions;
using FluentCsv.Tests.Results;
using Xunit;

namespace FluentCsv.Tests
{
    public class CsvFileParserShould
    {
        private readonly IDataSplitter _defaultDataSplitter = new SimpleDataSplitter();

        private CsvFileParser<TestResult> GetParser(string input, IDataSplitter splitter = null)
            => new CsvFileParser<TestResult>(input, splitter ?? _defaultDataSplitter, new CultureInfo("fr-FR"));

        [Fact]
        public void ParseSimpleCsvFromString()
        {
            const string input = "test1\r\nTest2\r\ntest3";

            var parser = GetParser(input);
            parser.AddColumn(0, a=>a.Member1);
            var result = parser.Parse(new ArrayCsvResult<TestResult>()).ResultSet;
            result.Should().HaveCount(3);            
        }

        [Fact]
        public void ParseCsvWithMultipleColumns()
        {
            const string input = "test1;1\r\ntest2;2\r\ntest3;3";

            var parser = GetParser(input);
            parser.AddColumn(0, a => a.Member1);
            parser.AddColumn(1, a => a.Member2);
            var result = parser.Parse(new ArrayCsvResult<TestResult>()).ResultSet;

            result.Should().HaveCount(3);
            result.ShouldContainEquivalentTo(
                TestResult.Create("test1", 1),
                TestResult.Create("test2", 2),
                TestResult.Create("test3", 3));
        }

        [Fact]
        public void DontParseFirstLineIfDeclaredHasHeader()
        {
            const string input = "Firstname;Lastname\r\nIven;Bazinet\r\nArridano;Charette\r\nOlivier;Gamelin";

            var parser = GetParser(input);
            parser.DeclareFirstLineHasHeader();
            parser.AddColumn(0,a=>a.Member1);

            var result = parser.Parse(new ArrayCsvResult<TestResult>()).ResultSet;

            result.Should().HaveCount(3);
            result[0].Member1.Should().Be("Iven");
            result[1].Member1.Should().Be("Arridano");
            result[2].Member1.Should().Be("Olivier");
        }

        [Fact]
        public void UseColumnName()
        {
            const string input = "Firstname;Age\r\nIven;20  \r\nArridano;30\r\nOlivier;40";

            var parser = GetParser(input);
            parser.AddColumn("Firstname", a => a.Member1);
            parser.AddColumn("Age", a => a.Member2);

            var result = parser.Parse(new ArrayCsvResult<TestResult>()).ResultSet;
            result.Should().HaveCount(3);
            result.ShouldContainEquivalentTo(
                TestResult.Create("Iven", 20),
                TestResult.Create("Arridano", 30),
                TestResult.Create("Olivier", 40));
        }

        [Fact]
        public void ThrowErrorIfUsingDuplicatedColumnName()
        {
            const string input = "FirstName;LastName;FirstName\r\nBenoit;DURANT;George";

            var parser = GetParser(input);
            Action action = () => parser.AddColumn("FirstName", a => a.Member1);

            action.Should().Throw<DuplicateColumnNameException>();
        }

        [Fact]
        public void ThrowErrorIfColumnNameDoesNotExists()
        {
            const string input = "FirstName;LastName\r\nBenoit;DURANT";

            var parser = GetParser(input);
            Action action = () => parser.AddColumn("NotExists", a => a.Member1);

            action.Should().Throw<ColumnNameNotFoundException>();
        }

        [Fact]
        public void DontThrowErrorIfNotUsingDuplicatedColumnName()
        {
            const string input = "FirstName;LastName;FirstName\r\nBenoit;DURANT;George";

            var parser = GetParser(input);
            parser.AddColumn("LastName", a => a.Member1);

            var result = parser.Parse(new ArrayCsvResult<TestResult>()).ResultSet;
            result.Should().HaveCount(1);
        }

        [Fact]
        public void DontIncludeExtraSpacesInHeader()
        {
            const string input = "FirstName   ;   LastName   \r\nBenoit;Durant";

            var parser = GetParser(input);
            parser.AddColumn("FirstName", a => a.Member1);            

            var result = parser.Parse(new ArrayCsvResult<TestResult>()).ResultSet;

            result.Should().HaveCount(1);            
            result.Should().AllBeEquivalentTo(TestResult.Create("Benoit"));
        }

        [Fact]
        public void ThrowErrorForEmptyLine()
        {
            const string input = "header1;header2\r\ntest1;test2\r\n\r\ntest3;test4";

            var parser = GetParser(input);
            parser.AddColumn("header1", a=>a.Member1);

            var result = parser.Parse(new ArrayCsvResult<TestResult>());
            result.Errors.Should().HaveCount(1);
            result.Errors.ShouldContainEquivalentTo(new CsvParseError(3,0,null,"The line is empty"));
        }

        [Fact]
        public void AcceptHeaderCaseUnsensitive()
        {
            const string input = "header1;header2\r\ntest1;test2\r\n\r\ntest3;test4";

            var parser = GetParser(input);
            parser.HeadersAsCaseInsensitive = true;
            parser.AddColumn("HEADER1", a => a.Member1);

            var result = parser.Parse(new ArrayCsvResult<TestResult>());
            result.ResultSet.ShouldContainEquivalentTo(
                TestResult.Create("test1"),
                TestResult.Create("test3"));
        }

        [Fact]
        public void WorkWithTuple() 
        {
	        const string input = "header1;header2\r\ntest1;test2\r\n\r\ntest3;test4";

            var parser = new CsvFileParser<(string h1, string h2)>(input, _defaultDataSplitter, new CultureInfo("fr-FR"));
            parser.AddColumn("header1", a=>a.h1);
            parser.AddColumn("header2", a=>a.h2);
            var result = parser.Parse(new ArrayCsvResult<(string h1, string h2)>());
            result.ResultSet.Length.Should().Be(2);
            result.ResultSet[0].h1.Should().Be("test1");
            result.ResultSet[0].h2.Should().Be("test2");
            result.ResultSet[1].h1.Should().Be("test3");
            result.ResultSet[1].h2.Should().Be("test4");
        }
    }
}