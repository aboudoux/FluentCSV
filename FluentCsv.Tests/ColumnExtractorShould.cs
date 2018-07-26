using System;
using FluentAssertions;
using FluentCsv.CsvParser;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    [TestFixture]
    public class ColumnExtractorShould
    {
        [Test]
        public void ExtractStringAndPutInField()
        {
            var result = new TestResult();
            var extractor = new ColumnExtractor<TestResult, string>(0);
            extractor.SetInto(a=>a.Member1);

            extractor.Extract(result,"bonjour");
            result.Member1.Should().Be("bonjour");
        }

        [Test]
        public void ExtractIntAndPutInField()
        {
            var result = new TestResult();
            var extractor = new ColumnExtractor<TestResult, int>(0);
            extractor.SetInto(a => a.Member2);

            extractor.Extract(result, "25");
            result.Member2.Should().Be(25);
        }

        [Test]
        public void ExtractDateAndPutInField()
        {
            var result = new TestResult();
            var extractor = new ColumnExtractor<TestResult, DateTime>(0);
            extractor.SetInto(a => a.Member3);

            extractor.Extract(result, "01/07/1980");
            result.Member3.Should().Be(new DateTime(1980,7,1));
        }

        [Test]
        [TestCase("25", 25)]
        [TestCase("", null)]
        public void ExtractNullableDecimalInField(string source, decimal? expected)
        {
            var result = new TestResult();
            var extractor = new ColumnExtractor<TestResult, decimal?>(0);
            extractor.SetInto(a => a.Member4);

            extractor.Extract(result, source);
            result.Member4.Should().Be(expected);
        }

        [Test]
        public void ExtractObjectInSpecialWay()
        {
            var result = new TestResult();
            var extractor = new ColumnExtractor<TestResult, string>(0);
            extractor.SetInto(a => a.Member1);
            extractor.SetInThisWay(a=>$"[TEST]{a}");

            extractor.Extract(result, "ESSAI");
            result.Member1.Should().Be("[TEST]ESSAI");
        }

        [Test]
        public void ExtractStringWithMultiline()
        {
            const string input = "test1\r\ntest2\r\ntest3";

            var result = new TestResult();
            var extractor = new ColumnExtractor<TestResult, string>(0);
            extractor.SetInto(a => a.Member1);

            extractor.Extract(result, input);
            result.Member1.Should().Be(input);
        }
    }
}