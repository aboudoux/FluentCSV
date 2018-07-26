using System;
using FluentAssertions;
using FluentCsv.CsvParser;
using FluentCsv.Exceptions;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    public class CsvFileParserShould
    {
        [Test]
        public void ParseSimpleCsvFromString()
        {
            const string input = "test1\r\nTest2\r\ntest3";

            var parser = new CsvFileParser<TestResult>(input);
            parser.AddColumn(0, a=>a.Member1);
            var result = parser.Parse().ResultSet;
            result.Should().HaveCount(3);            
        }

        [Test]
        public void ParseCsvWithMultipleColumns()
        {
            const string input = "test1;1\r\ntest2;2\r\ntest3;3";

            var parser = new CsvFileParser<TestResult>(input);
            parser.AddColumn(0, a => a.Member1);
            parser.AddColumn(1, a => a.Member2);
            var result = parser.Parse().ResultSet;

            result.Should().HaveCount(3);
            result.ShouldContainEquivalentTo(
                TestResult.Create("test1", 1),
                TestResult.Create("test2", 2),
                TestResult.Create("test3", 3));
        }

        [Test]
        public void DontParseFirstLineIfDeclaredHasHeader()
        {
            const string input = "Firstname;Lastname\r\nIven;Bazinet\r\nArridano;Charette\r\nOlivier;Gamelin";

            var parser = new CsvFileParser<TestResult>(input);
            parser.DeclareFirstLineHasHeader();
            parser.AddColumn(0,a=>a.Member1);

            var result = parser.Parse().ResultSet;

            result.Should().HaveCount(3);
            result[0].Member1.Should().Be("Iven");
            result[1].Member1.Should().Be("Arridano");
            result[2].Member1.Should().Be("Olivier");
        }

        [Test]
        public void UseColumnName()
        {
            const string input = "Firstname;Age\r\nIven;20  \r\nArridano;30\r\nOlivier;40";

            var parser = new CsvFileParser<TestResult>(input);
            parser.AddColumn("Firstname", a => a.Member1);
            parser.AddColumn("Age", a => a.Member2);

            var result = parser.Parse().ResultSet;
            result.Should().HaveCount(3);
            result.ShouldContainEquivalentTo(
                TestResult.Create("Iven", 20),
                TestResult.Create("Arridano", 30),
                TestResult.Create("Olivier", 40));
        }

        [Test]
        public void ThrowErrorIfUsingDuplicatedColumnName()
        {
            const string input = "FirstName;LastName;FirstName\r\nBenoit;DURANT;George";

            var parser = new CsvFileParser<TestResult>(input);
            Action action = () => parser.AddColumn("FirstName", a => a.Member1);

            action.Should().Throw<DuplicateColumnNameException>();
        }

        [Test]
        public void ThrowErrorIfColumnNameDoesNotExists()
        {
            const string input = "FirstName;LastName\r\nBenoit;DURANT";

            var parser = new CsvFileParser<TestResult>(input);
            Action action = () => parser.AddColumn("NotExists", a => a.Member1);

            action.Should().Throw<ColumnNameNotFoundException>();
        }

        [Test]
        public void DontThrowErrorIfNotUsingDuplicatedColumnName()
        {
            const string input = "FirstName;LastName;FirstName\r\nBenoit;DURANT;George";

            var parser = new CsvFileParser<TestResult>(input);
            parser.AddColumn("LastName", a => a.Member1);

            var result = parser.Parse().ResultSet;
            result.Should().HaveCount(1);
        }

        [Test]
        public void DontIncludeExtraSpacesInHeader()
        {
            const string input = "FirstName   ;   LastName   \r\nBenoit;Durant";

            var parser = new CsvFileParser<TestResult>(input);
            parser.AddColumn("FirstName", a => a.Member1);            

            var result = parser.Parse().ResultSet;

            result.Should().HaveCount(1);            
            result.Should().AllBeEquivalentTo(TestResult.Create("Benoit"));
        }
    }
}