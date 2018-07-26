using System;
using FluentCsv.CsvParser;
using FluentCsv.FluentReader;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    public class FluentCsvReadWithErrorShould
    {
        [Test]
        public void Test()
        {
            const string input = "Header\r\ntest1\r\ntest2\r\nbad\r\ntest3";

            var result = Read.Csv.FromString(input)
                .That.ReturnsLinesOf<TestResult>()
                .Put.Column("Header").InThisWay(CheckBadString).Into(a => a.Member1)
                .GetAll();

            result.ResultSet.ShouldContainEquivalentTo(
                TestResult.Create("test1"),
                TestResult.Create("test2"),
                TestResult.Create("test3"));

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