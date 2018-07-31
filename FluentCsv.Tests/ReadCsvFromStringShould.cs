using System;
using System.Globalization;
using FluentAssertions;
using FluentCsv.Exceptions;
using FluentCsv.FluentReader;
using FluentCsv.Tests.Results;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    public class ReadCsvFromStringShould
    {
        [Test]
        public void ReturnsSimpleResultSet()
        {
            const string input = "test1;1\r\ntest2;2\r\ntest3;3";

            var resultSet = Read.Csv.FromString(input)
                .That.ReturnsLinesOf<TestResult>()
                .Put.Column(0).Into(result => result.Member1)
                .Put.Column(1).As<int>().Into(result => result.Member2)
                .GetAll().ResultSet;

            resultSet.Should().HaveCount(3);
            resultSet.ShouldContainEquivalentTo(
                TestResult.Create("test1", 1),
                TestResult.Create("test2", 2),
                TestResult.Create("test3", 3));
        }

        [Test]
        public void ReturnResultSetWithColumnTransformation()
        {
            const string input = "01012001\r\n01022002\r\n01032003";

            var resultSet = Read.Csv.FromString(input)
                .That.ReturnsLinesOf<TestResult>()
                .Put.Column(0).As<DateTime>().InThisWay(ParseFromEightDigit).Into(m => m.Member3)
                .GetAll().ResultSet;

            resultSet.Should().HaveCount(3);
            resultSet.ShouldContainEquivalentTo(
                TestResult.Create(member3: new DateTime(2001, 01, 01)),
                TestResult.Create(member3: new DateTime(2002, 02, 01)),
                TestResult.Create(member3: new DateTime(2003, 03, 01))
            );
        }

        [Test]
        public void ParseCsvWithCustomColumnSeparator()
        {
            const string input = "test1<->1<->01012001\r\ntest2<->2<->01012002\r\ntest3<->3<->01012003";

            var resultSet = Read.Csv.FromString(input)
                .Where.ColumnsAreDelimitedBy("<->")
                .That.ReturnsLinesOf<TestResult>()
                .Put.Column(0).Into(a => a.Member1)
                .Put.Column(1).As<int>().Into(a => a.Member2)
                .Put.Column(2).As<DateTime>().InThisWay(ParseFromEightDigit).Into(a => a.Member3)
                .GetAll().ResultSet;

            resultSet.Should().HaveCount(3);
            resultSet.ShouldContainEquivalentTo(
                TestResult.Create("test1", 1, new DateTime(2001, 01, 01)),
                TestResult.Create("test2", 2, new DateTime(2002, 01, 01)),
                TestResult.Create("test3", 3, new DateTime(2003, 01, 01))
            );
        }

        [Test]
        public void ParseWithCustomLineSeparator()
        {
            const string input = "test1<endl>test2<endl>test3";

            var resultSet = Read.Csv.FromString(input)
                .Where.LinesEndWith("<endl>")
                .That.ReturnsLinesOf<TestResult>()
                .Put.Column(0).Into(a => a.Member1)
                .GetAll().ResultSet;

            resultSet.Should().HaveCount(3);
            resultSet[0].Member1.Should().Be("test1");
            resultSet[1].Member1.Should().Be("test2");
            resultSet[2].Member1.Should().Be("test3");
        }

        [Test]
        public void ParseWithCustomLineAndColumnSeparator()
        {
            const string input = @"test1-1-01012001^test2-2-01012002^test3-3-01012003";

            var resultSet = Read.Csv.FromString(input)
                .Where.LinesEndWith("^").And.ColumnsAreDelimitedBy("-")
                .That.ReturnsLinesOf<TestResult>()
                .Put.Column(0).Into(a => a.Member1)
                .Put.Column(1).As<int>().Into(a => a.Member2)
                .Put.Column(2).As<DateTime>().InThisWay(ParseFromEightDigit).Into(a => a.Member3)
                .GetAll().ResultSet;

            resultSet.Should().HaveCount(3);
            resultSet.ShouldContainEquivalentTo(
                TestResult.Create("test1", 1, new DateTime(2001, 01, 01)),
                TestResult.Create("test2", 2, new DateTime(2002, 01, 01)),
                TestResult.Create("test3", 3, new DateTime(2003, 01, 01))
                );
        }

        [Test]
        public void DontReadFirstLineIfHeader()
        {
            const string input = "Name,Age\r\nJules,20\r\nGaetan,30\r\nBenoit,40";

            var resultset = Read.Csv.FromString(input)
                .Where.ColumnsAreDelimitedBy(",").And.FirstLineIsHeader()
                .That.ReturnsLinesOf<TestResult>()
                .Put.Column(0).Into(a => a.Member1)
                .Put.Column(1).As<int>().Into(a => a.Member2)
                .GetAll().ResultSet;

            resultset.Should().HaveCount(3);      
            resultset.ShouldContainEquivalentTo(
                TestResult.Create("Jules", 20), 
                TestResult.Create("Gaetan", 30),
                TestResult.Create("Benoit", 40));
        }

        [Test]
        public void UseColumnName()
        {
            const string input = "Name,Age\r\nJules,20\r\nGaetan,30\r\nBenoit,40";

            var resultset = Read.Csv.FromString(input)
                .Where.ColumnsAreDelimitedBy(",").And.FirstLineIsHeader()
                .That.ReturnsLinesOf<TestResult>()
                .Put.Column("Name").Into(a => a.Member1)
                .Put.Column("Age").As<int>().Into(a => a.Member2)
                .GetAll().ResultSet;

            resultset.Should().HaveCount(3);        
            resultset.ShouldContainEquivalentTo(
                TestResult.Create("Jules", 20), 
                TestResult.Create("Gaetan", 30),
                TestResult.Create("Benoit", 40));
        }

        private static DateTime ParseFromEightDigit(string eightDigitDate)
            => DateTime.ParseExact(eightDigitDate, "ddMMyyyy", CultureInfo.InvariantCulture);


        [Test]
        public void ReadColumnWithColumnSeparator()
        {
            const string input = "Firstname;Lastname;Address\r\nAurelien;BOUDOUX;\"9 rue du test;impasse;75001;Paris\"";

            var resultSet = Read.Csv.FromString(input)
                .That.ReturnsLinesOf<TestResultWithMultiline>()
                .Put.Column("Firstname").Into(a => a.Firstname)
                .Put.Column("Lastname").Into(a => a.Lastname)
                .Put.Column("Address").Into(a => a.Address)
                .GetAll().ResultSet;

            resultSet.ShouldContainEquivalentTo(TestResultWithMultiline.Create("Aurelien", "BOUDOUX", "9 rue du test;impasse;75001;Paris"));
        }

        [Test]
        public void DontUseRfc4180ForParsing()
        {
            const string input = "Firstname;Lastname\r\n\"M. B\"OK;Benoit";

            var resultSet = Read.Csv.FromString(input)
                .Where.Rfc4180IsNotUsedForParsing()
                .That.ReturnsLinesOf<TestResultWithMultiline>()
                .Put.Column("Firstname").Into(a => a.Firstname)
                .Put.Column("Lastname").Into(a => a.Lastname)
                .GetAll().ResultSet;

            resultSet.ShouldContainEquivalentTo(TestResultWithMultiline.Create("\"M. B\"OK", "Benoit"));
        }

        [Test]
        public void ReadColumnWithMultilineAndColumnSeparator()
        {
            const string input = "Firstname;Lastname;Address\r\nAurelien;BOUDOUX;\"9\r\nrue du test; impasse\r\n75001\r\nParis\"";

            var resultSet = Read.Csv.FromString(input)
                .That.ReturnsLinesOf<TestResultWithMultiline>()
                .Put.Column("Firstname").Into(a => a.Firstname)
                .Put.Column("Lastname").Into(a => a.Lastname)
                .Put.Column("Address").Into(a => a.Address)
                .GetAll().ResultSet;

            resultSet.ShouldContainEquivalentTo(TestResultWithMultiline.Create("Aurelien","BOUDOUX", "9\r\nrue du test; impasse\r\n75001\r\nParis"));
        }

        [Test]
        public void ReadCsvWithColumnThatContainsNewLine()
        {
            const string input = "\"FirstName\r\nLastName\";\"Home\r\nAddress\"\r\nMARTIN;\"12\r\nRue test\"";

            var resultSet = Read.Csv.FromString(input)
                .That.ReturnsLinesOf<TestResultWithMultiline>()
                .Put.Column("FirstName\r\nLastName").Into(a => a.Firstname)
                .Put.Column("Home\r\nAddress").Into(a => a.Address)
                .GetAll().ResultSet;

            resultSet.ShouldContainEquivalentTo(TestResultWithMultiline.Create("MARTIN", address: "12\r\nRue test"));
        }

        [Test]
        public void ThrowErrorIfUsingQuoteAsLineDelimiterInRfc4810ButWorksIfDisableIt()
        {
            const string input = "Header1;Header2\"Value1;Value2\"Value3;Value4";

            Action action = () => Read.Csv.FromString(input)
                .Where.LinesEndWith("\"")
                .That.ReturnsLinesOf<TestResultWithMultiline>()
                .Put.Column("Header1").Into(a => a.Firstname)
                .Put.Column("Header2").Into(a => a.Lastname)
                .GetAll();

            action.Should().Throw<BadDelimiterException>();

            var result = Read.Csv.FromString(input)
                .Where.LinesEndWith("\"").And.Rfc4180IsNotUsedForParsing()
                .That.ReturnsLinesOf<TestResultWithMultiline>()
                .Put.Column("Header1").Into(a => a.Firstname)
                .Put.Column("Header2").Into(a => a.Lastname)
                .GetAll().ResultSet;

            result.Should().HaveCount(2);
            result.ShouldContainEquivalentTo(
                TestResultWithMultiline.Create("Value1", "Value2"),
                TestResultWithMultiline.Create("Value3", "Value4"));
        }

        [Test]
        public void ThrowErrorIfUsingQuoteAsColumnsDelimiterInRfc4810ButWorksIfDisableIt()
        {
            const string input = "Header1\"Header2\r\nValue1\"Value2\r\nValue3\"Value4";

            Action action = () => Read.Csv.FromString(input)
                .Where.ColumnsAreDelimitedBy("\"")
                .That.ReturnsLinesOf<TestResultWithMultiline>()
                .Put.Column("Header1").Into(a => a.Firstname)
                .Put.Column("Header2").Into(a => a.Lastname)
                .GetAll();

            action.Should().Throw<BadDelimiterException>();

            var result = Read.Csv.FromString(input)
                .Where.ColumnsAreDelimitedBy("\"").And.Rfc4180IsNotUsedForParsing()
                .That.ReturnsLinesOf<TestResultWithMultiline>()
                .Put.Column("Header1").Into(a => a.Firstname)
                .Put.Column("Header2").Into(a => a.Lastname)
                .GetAll().ResultSet;

            result.Should().HaveCount(2);
            result.ShouldContainEquivalentTo(
                TestResultWithMultiline.Create("Value1", "Value2"),
                TestResultWithMultiline.Create("Value3", "Value4"));
        }
    }
}