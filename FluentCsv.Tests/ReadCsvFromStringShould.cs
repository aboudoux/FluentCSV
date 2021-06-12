using System;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using FluentCsv.CsvParser;
using FluentCsv.Exceptions;
using FluentCsv.FluentReader;
using FluentCsv.Tests.Results;
using Xunit;

namespace FluentCsv.Tests
{
    public class ReadCsvFromStringShould
    {
        [Fact]
        public void ReturnsSimpleResultSet()
        {
            const string input = "test1;1\r\ntest2;2\r\ntest3;3";

            var resultSet = Read.Csv.FromString(input)
                .ThatReturns.ArrayOf<TestResult>()
                .Put.Column(0).Into(result => result.Member1)
                .Put.Column(1).As<int>().Into(result => result.Member2)
                .GetAll().ResultSet;

            resultSet.Should().HaveCount(3);
            resultSet.ShouldContainEquivalentTo(
                TestResult.Create("test1", 1),
                TestResult.Create("test2", 2),
                TestResult.Create("test3", 3));
        }

        [Fact]
        public void ReturnResultSetWithColumnTransformation()
        {
            const string input = "01012001\r\n01022002\r\n01032003";

            var resultSet = Read.Csv.FromString(input)
                .ThatReturns.ArrayOf<TestResult>()
                .Put.Column(0).As<DateTime>().InThisWay(ParseFromEightDigit).Into(m => m.Member3)
                .GetAll().ResultSet;

            resultSet.Should().HaveCount(3);
            resultSet.ShouldContainEquivalentTo(
                TestResult.Create(member3: new DateTime(2001, 01, 01)),
                TestResult.Create(member3: new DateTime(2002, 02, 01)),
                TestResult.Create(member3: new DateTime(2003, 03, 01))
            );
        }

        [Fact]
        public void ParseCsvWithCustomColumnSeparator()
        {
            const string input = "test1<->1<->01012001\r\ntest2<->2<->01012002\r\ntest3<->3<->01012003";

            var resultSet = Read.Csv.FromString(input)
                .With.ColumnsDelimiter("<->")
                .ThatReturns.ArrayOf<TestResult>()
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

        [Fact]
        public void ParseWithCustomLineSeparator()
        {
            const string input = "test1<endl>test2<endl>test3";

            var resultSet = Read.Csv.FromString(input)
                .With.EndOfLineDelimiter("<endl>")
                .ThatReturns.ArrayOf<TestResult>()
                .Put.Column(0).Into(a => a.Member1)
                .GetAll().ResultSet;

            resultSet.Should().HaveCount(3);
            resultSet[0].Member1.Should().Be("test1");
            resultSet[1].Member1.Should().Be("test2");
            resultSet[2].Member1.Should().Be("test3");
        }

        [Fact]
        public void ParseWithCustomLineAndColumnSeparator()
        {
            const string input = @"test1-1-01012001^test2-2-01012002^test3-3-01012003";

            var resultSet = Read.Csv.FromString(input)
                .With.EndOfLineDelimiter("^").And.ColumnsDelimiter("-")
                .ThatReturns.ArrayOf<TestResult>()
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

        [Fact]
        public void DontReadFirstLineIfHeader()
        {
            const string input = "Name,Age\r\nJules,20\r\nGaetan,30\r\nBenoit,40";

            var resultset = Read.Csv.FromString(input)
                .With.ColumnsDelimiter(",").And.Header()
                .ThatReturns.ArrayOf<TestResult>()
                .Put.Column(0).Into(a => a.Member1)
                .Put.Column(1).As<int>().Into(a => a.Member2)
                .GetAll().ResultSet;

            resultset.Should().HaveCount(3);      
            resultset.ShouldContainEquivalentTo(
                TestResult.Create("Jules", 20), 
                TestResult.Create("Gaetan", 30),
                TestResult.Create("Benoit", 40));
        }

        [Fact]
        public void UseColumnName()
        {
            const string input = "Name,Age\r\nJules,20\r\nGaetan,30\r\nBenoit,40";

            var resultset = Read.Csv.FromString(input)
                .With.ColumnsDelimiter(",").And.Header()
                .ThatReturns.ArrayOf<TestResult>()
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

        [Fact]
        public void ReadColumnWithColumnSeparator()
        {
            const string input = "Firstname;Lastname;Address\r\nAurelien;BOUDOUX;\"9 rue du test;impasse;75001;Paris\"";

            var resultSet = Read.Csv.FromString(input)
                .ThatReturns.ArrayOf<TestResultWithMultiline>()
                .Put.Column("Firstname").Into(a => a.Firstname)
                .Put.Column("Lastname").Into(a => a.Lastname)
                .Put.Column("Address").Into(a => a.Address)
                .GetAll().ResultSet;

            resultSet.ShouldContainEquivalentTo(TestResultWithMultiline.Create("Aurelien", "BOUDOUX", "9 rue du test;impasse;75001;Paris"));
        }

        [Fact]
        public void DontUseRfc4180ForParsing()
        {
            const string input = "Firstname;Lastname\r\n\"M. B\"OK;Benoit";

            var resultSet = Read.Csv.FromString(input)
                .With.SimpleParsingMode()
                .ThatReturns.ArrayOf<TestResultWithMultiline>()
                .Put.Column("Firstname").Into(a => a.Firstname)
                .Put.Column("Lastname").Into(a => a.Lastname)
                .GetAll().ResultSet;

            resultSet.ShouldContainEquivalentTo(TestResultWithMultiline.Create("\"M. B\"OK", "Benoit"));
        }

        [Fact]
        public void ReadColumnWithMultilineAndColumnSeparator()
        {
            const string input = "Firstname;Lastname;Address\r\nAurelien;BOUDOUX;\"9\r\nrue du test; impasse\r\n75001\r\nParis\"";

            var resultSet = Read.Csv.FromString(input)
                .ThatReturns.ArrayOf<TestResultWithMultiline>()
                .Put.Column("Firstname").Into(a => a.Firstname)
                .Put.Column("Lastname").Into(a => a.Lastname)
                .Put.Column("Address").Into(a => a.Address)
                .GetAll().ResultSet;

            resultSet.ShouldContainEquivalentTo(TestResultWithMultiline.Create("Aurelien","BOUDOUX", "9\r\nrue du test; impasse\r\n75001\r\nParis"));
        }

        [Fact]
        public void ReadCsvWithColumnThatContainsNewLine()
        {
            const string input = "\"FirstName\r\nLastName\";\"Home\r\nAddress\"\r\nMARTIN;\"12\r\nRue test\"";

            var resultSet = Read.Csv.FromString(input)
                .ThatReturns.ArrayOf<TestResultWithMultiline>()
                .Put.Column("FirstName\r\nLastName").Into(a => a.Firstname)
                .Put.Column("Home\r\nAddress").Into(a => a.Address)
                .GetAll().ResultSet;

            resultSet.ShouldContainEquivalentTo(TestResultWithMultiline.Create("MARTIN", address: "12\r\nRue test"));
        }

        [Fact]
        public void ThrowErrorIfUsingQuoteAsLineDelimiterInRfc4810ButWorksIfDisableIt()
        {
            const string input = "Header1;Header2\"Value1;Value2\"Value3;Value4";

            Action action = () => Read.Csv.FromString(input)
                .With.EndOfLineDelimiter("\"")
                .ThatReturns.ArrayOf<TestResultWithMultiline>()
                .Put.Column("Header1").Into(a => a.Firstname)
                .Put.Column("Header2").Into(a => a.Lastname)
                .GetAll();

            action.Should().Throw<BadDelimiterException>();

            var result = Read.Csv.FromString(input)
                .With.EndOfLineDelimiter("\"").And.SimpleParsingMode()
                .ThatReturns.ArrayOf<TestResultWithMultiline>()
                .Put.Column("Header1").Into(a => a.Firstname)
                .Put.Column("Header2").Into(a => a.Lastname)
                .GetAll().ResultSet;

            result.Should().HaveCount(2);
            result.ShouldContainEquivalentTo(
                TestResultWithMultiline.Create("Value1", "Value2"),
                TestResultWithMultiline.Create("Value3", "Value4"));
        }

        [Fact]
        public void ThrowErrorIfUsingQuoteAsColumnsDelimiterInRfc4810ButWorksIfDisableIt()
        {
            const string input = "Header1\"Header2\r\nValue1\"Value2\r\nValue3\"Value4";

            Action action = () => Read.Csv.FromString(input)
                .With.ColumnsDelimiter("\"")
                .ThatReturns.ArrayOf<TestResultWithMultiline>()
                .Put.Column("Header1").Into(a => a.Firstname)
                .Put.Column("Header2").Into(a => a.Lastname)
                .GetAll();

            action.Should().Throw<BadDelimiterException>();

            var result = Read.Csv.FromString(input)
                .With.ColumnsDelimiter("\"").And.SimpleParsingMode()
                .ThatReturns.ArrayOf<TestResultWithMultiline>()
                .Put.Column("Header1").Into(a => a.Firstname)
                .Put.Column("Header2").Into(a => a.Lastname)
                .GetAll().ResultSet;

            result.Should().HaveCount(2);
            result.ShouldContainEquivalentTo(
                TestResultWithMultiline.Create("Value1", "Value2"),
                TestResultWithMultiline.Create("Value3", "Value4"));
        }

        [Fact]
        public void ThrowErrorIfDefineEmptyColumnsDelimiter()
        {
            const string input = "Header1\"Header2\r\nValue1\"Value2\r\nValue3\"Value4";

            Action action = () => Read.Csv.FromString(input)
                .With.ColumnsDelimiter(string.Empty)
                .ThatReturns.ArrayOf<TestResultWithMultiline>()
                .Put.Column("Header1").Into(a => a.Firstname)
                .Put.Column("Header2").Into(a => a.Lastname)
                .GetAll();

            action.Should().Throw<EmptyColumnDelimiterException>();
        }

        [Fact]
        public void ThrowErrorIfDefineEmptyLinesDelimiter()
        {
            const string input = "Header1\"Header2\r\nValue1\"Value2\r\nValue3\"Value4";

            Action action = () => Read.Csv.FromString(input)
                .With.EndOfLineDelimiter(string.Empty)
                .ThatReturns.ArrayOf<TestResultWithMultiline>()
                .Put.Column("Header1").Into(a => a.Firstname)
                .Put.Column("Header2").Into(a => a.Lastname)
                .GetAll();

            action.Should().Throw<EmptyLineDelimiterException>();
        }

        [Fact]
        public void AllowWhiteSpaceInLineDelimiter()
        {
            const string input = "Header1;Header2 Value1;Value2 Value3;Value4";

            var result = Read.Csv.FromString(input)
                .With.EndOfLineDelimiter(" ")
                .ThatReturns.ArrayOf<TestResultWithMultiline>()
                .Put.Column("Header1").Into(a => a.Firstname)
                .Put.Column("Header2").Into(a => a.Lastname)
                .GetAll().ResultSet;

            result.Should().HaveCount(2);
            result.ShouldContainEquivalentTo(
                TestResultWithMultiline.Create("Value1", "Value2"),
                TestResultWithMultiline.Create("Value3", "Value4"));
        }

        [Fact]
        public void AllowWhiteSpaceInColumnsLineDelimiter()
        {
            const string input = "Header1 Header2\r\nValue1 Value2\r\nValue3 Value4";

            var result = Read.Csv.FromString(input)
                .With.ColumnsDelimiter(" ")
                .ThatReturns.ArrayOf<TestResultWithMultiline>()
                .Put.Column("Header1").Into(a => a.Firstname)
                .Put.Column("Header2").Into(a => a.Lastname)
                .GetAll().ResultSet;

            result.Should().HaveCount(2);
            result.ShouldContainEquivalentTo(
                TestResultWithMultiline.Create("Value1", "Value2"),
                TestResultWithMultiline.Create("Value3", "Value4"));
        }

        [Fact]
        public void ReadHeaderCaseSensitive()
        {
            const string input = "HeAder1\r\ntest1";

            Action action = () => Read.Csv.FromString(input)
                .With.Header(As.CaseSensitive)
                .ThatReturns.ArrayOf<TestResult>()
                .Put.Column("header1").Into(a => a.Member1)
                .GetAll();

            action.Should().Throw<ColumnNameNotFoundException>();
        }

        [Fact]
        public void ThrowErrorIfDuplicateCaseInsensitiveHeader()
        {
            const string input = "header;HEADER\r\ntest;TEST";

            Action action = () => Read.Csv.FromString(input)
                .ThatReturns.ArrayOf<TestResult>()
                .Put.Column("header").Into(a => a.Member1)
                .GetAll();

            action.Should().Throw<DuplicateColumnNameException>();
        }

        [Fact]
        public void DontThrowErrorIfDuplicateCaseSensitiveHeader()
        {
            const string input = "header;HEADER\r\ntest1;TEST2";

            var result = Read.Csv.FromString(input)
                .With.Header(As.CaseSensitive)
                .ThatReturns.ArrayOf<TestResult>()
                .Put.Column("header").Into(a => a.Member1)
                .GetAll().ResultSet;

            result.ShouldContainEquivalentTo(TestResult.Create("test1"));
        }

        [Fact]
        public void WorkWithDeepStructureThatContainsValueObject()
        {
            const string input = "Name;Phone\r\nDUPONT;0187225445\r\nMARTIN;0655457676\r\nERROR;078872129\r\n";

            var result = Read.Csv.FromString(input)
                .ThatReturns.ArrayOf<ResultWithDeepValueObject>()
                .Put.Column("name").Into(a => a.Contact.Name)
                .Put.Column("phone").As<Phone>().InThisWay(phone => new Phone(phone)).Into(r => r.Contact.Phone)
                .GetAll();

            result.Errors.Should().HaveCount(1);
            result.ResultSet.Should().HaveCount(2);

            result.Errors.ShouldContainEquivalentTo(new CsvParseError(4,1,"phone", "078872129 is not a valid phone number"));
        }

        [Fact]
        public void ResolveCsvDataWithUsCulture()
        {
            const string input = "Date;Size\r\n07/01/2018;1.2";

            var csv = Read.Csv.FromString(input)
                .With.CultureInfo("en-US")
                .ThatReturns.ArrayOf<TestResult>()
                .Put.Column("date").As<DateTime>().Into(a => a.Member3)
                .Put.Column("size").As<decimal?>().Into(a => a.Member4)
                .GetAll();

            csv.ResultSet.ShouldContainEquivalentTo(TestResult.Create(member3: new DateTime(2018,07,01), member4: 1.2m));
        }

		[Theory]
		[InlineData("\uFEFFHEADER1;HEADER2;HEADER3\r\nTEST1;TEST2;TEST3")]
		[InlineData("\uFEFF\"HEADER1\";\"HEADER2\";\"HEADER3\"\r\nTEST1;TEST2;TEST3")]
	    public void ReadUtf8StringWithBom(string csvData)
		{
			var csv = Read.Csv.FromString(csvData)
				.ThatReturns.ArrayOf<TestResult>()
				.Put.Column("HEADER1").Into(a => a.Member1)
				.GetAll();

			csv.ResultSet.Should().HaveCount(1);
			csv.ResultSet.First().Member1.Should().Be("TEST1");
		}
    }
}