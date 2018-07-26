using System;
using System.Globalization;
using FluentAssertions;
using FluentCsv.FluentReader;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    public class ReadCsvFromStringShould
    {
        [Test]
        public void ReturnsSimpleResultSet()
        {
            const string csv = "test1;1\r\ntest2;2\r\ntest3;3";

            var resultSet = Read.Csv.FromString(csv)
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
            const string csv = "01012001\r\n01022002\r\n01032003";

            var resultSet = Read.Csv.FromString(csv)
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
            const string csv = "test1<->1<->01012001\r\ntest2<->2<->01012002\r\ntest3<->3<->01012003";

            var resultSet = Read.Csv.FromString(csv)
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
            const string csv = "test1<endl>test2<endl>test3";

            var resultSet = Read.Csv.FromString(csv)
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
            const string csv = @"test1-1-01012001^test2-2-01012002^test3-3-01012003";

            var resultSet = Read.Csv.FromString(csv)
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
            const string csv = "Name,Age\r\nJules,20\r\nGaetan,30\r\nBenoit,40";

            var resultset = Read.Csv.FromString(csv)
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
            const string csv = "Name,Age\r\nJules,20\r\nGaetan,30\r\nBenoit,40";

            var resultset = Read.Csv.FromString(csv)
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
    }
}