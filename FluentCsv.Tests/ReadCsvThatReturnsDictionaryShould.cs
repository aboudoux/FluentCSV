using System;
using FluentAssertions;
using FluentCsv.Exceptions;
using FluentCsv.FluentReader;
using FluentCsv.Tests.Results;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    public class ReadCsvThatReturnsDictionaryShould
    {
        [Test]
        public void CreateDictionaryOfStringWithoutError()
        {
            const string input = "C1;C2\r\nA;1\r\nB;2\r\nC;3";

            var csv = Read.Csv.FromString(input)
                .ThatReturns.DictionaryOf<TestResult>(a => a.Member1)
                .Put.Column("C1").Into(a => a.Member1)
                .Put.Column("C2").As<int>().Into(a => a.Member2)
                .GetAll();

            csv.ResultSet.Should().ContainKey("A");
            csv.ResultSet["B"].Member2.Should().Be(2);
        }

        [Test]
        public void CreateDictionaryOfIntWithoutError()
        {
            const string input = "C1;C2\r\nA;1\r\nB;2\r\nC;3";

            var csv = Read.Csv.FromString(input)
                .ThatReturns.DictionaryOf<TestResult>(a => a.Member2)
                .Put.Column("C1").Into(a => a.Member1)
                .Put.Column("C2").As<int>().Into(a => a.Member2)
                .GetAll();

            csv.ResultSet.Should().ContainKey(1);
        }

        [Test]
        public void ThrowErrorIfDuplicateKeyAndShowLineNumberWithHeader()
        {
            const string input = "C1;C2\r\nA;1\r\nB;2\r\nA;3";

            Action action = () => Read.Csv.FromString(input)
                .ThatReturns.DictionaryOf<TestResult>(a => a.Member1)
                .Put.Column("C1").Into(a => a.Member1)
                .Put.Column("C2").As<int>().Into(a => a.Member2)
                .GetAll();

            action.Should().Throw<DuplicateKeyException>()
                .And.Message.Should().Be("The key 'A' already exists.");
        }
    }
}