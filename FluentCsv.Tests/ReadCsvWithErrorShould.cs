using System;
using FluentAssertions;
using FluentCsv.CsvParser;
using FluentCsv.Exceptions;
using FluentCsv.FluentReader;
using FluentCsv.Tests.Results;
using Xunit;

namespace FluentCsv.Tests
{
    public class ReadCsvWithErrorShould
    {
        [Fact]
        public void ThrowFluentCsvExceptionIfHeaderNotExists()
        {
            Action action = () =>
             Read.Csv.FromString("")
                .ThatReturns.ArrayOf<TestResult>()
                .Put.Column("UnknowHeader").Into(a => a.Member1)
                .GetAll();

            action.Should().Throw<FluentCsvException>();
        }

        [Fact]
        public void ReturnProperErrorIfColumnIndexNotFound()
        {
            var result = Read.Csv.FromString("test;test2")
                .ThatReturns.ArrayOf<TestResult>()
                .Put.Column(5).Into(a => a.Member1)
                .GetAll();

            result.Errors.ShouldContainEquivalentTo(new CsvParseError(1,5,null, "The column at index 5 does not exists for line number 1"));
        }


        [Fact]
        public void ContainsCustomErrorIfBadStringFound()
        {
            const string input = "Header\r\ntest1\r\ntest2\r\nbad\r\ntest3";

            var result = Read.Csv.FromString(input)
                .ThatReturns.ArrayOf<TestResult>()
                .Put.Column("Header").InThisWay(CheckBadString).Into(a => a.Member1)
                .GetAll();

            result.ResultSet.ShouldContainEquivalentTo(
                TestResult.Create("test1"),
                TestResult.Create("test2"),
                TestResult.Create("test3"));

            result.Errors.ShouldContainEquivalentTo(new CsvParseError(4, 0, "Header", "bad string found"));

            string CheckBadString(string source)
            {
                if (source == "bad")
                    throw new Exception("bad string found");
                return source;
            }
        }
    }
}