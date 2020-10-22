using System;
using System.Globalization;
using FluentAssertions;
using FluentCsv.CsvParser;
using FluentCsv.Tests.Results;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    [TestFixture]
    public class ColumnExtractorShould
    {
        [Test]
        public void ExtractStringAndPutInField()
        {
            var source = new TestResult();
            var result = source as object;
            var extractor = new ColumnExtractor<TestResult, string>(0, CultureInfo.CurrentCulture);
            extractor.SetInto(a=>a.Member1);

            extractor.Extract(source,"bonjour", out result);
            source.Member1.Should().Be("bonjour");
        }

        /*
        [Test]
        public void ExtractIntAndPutInField()
        {
            var result = new TestResult();
            var extractor = new ColumnExtractor<TestResult, int>(0, CultureInfo.CurrentCulture);
            extractor.SetInto(a => a.Member2);

            extractor.Extract(result, "25", out result);
            result.Member2.Should().Be(25);
        }

        [Test]
        public void ExtractDateAndPutInField()
        {
            var result = new TestResult();
            var extractor = new ColumnExtractor<TestResult, DateTime>(0, CultureInfo.CurrentCulture);
            extractor.SetInto(a => a.Member3);

            extractor.Extract(result, "01/07/1980", out result);
            result.Member3.Should().Be(new DateTime(1980,7,1));
        }

        [Test]
        [TestCase("25", 25)]
        [TestCase("", null)]
        public void ExtractNullableDecimalInField(string source, decimal? expected)
        {
            var result = new TestResult();
            var extractor = new ColumnExtractor<TestResult, decimal?>(0, CultureInfo.CurrentCulture);
            extractor.SetInto(a => a.Member4);

            extractor.Extract(result, source, out result);
            result.Member4.Should().Be(expected);
        }

        [Test]
        public void ExtractObjectInSpecialWay()
        {
            var result = new TestResult();
            var extractor = new ColumnExtractor<TestResult, string>(0, CultureInfo.CurrentCulture);
            extractor.SetInto(a => a.Member1);
            extractor.SetInThisWay(a=>$"[TEST]{a}");

            extractor.Extract(result, "ESSAI", out result);
            result.Member1.Should().Be("[TEST]ESSAI");
        }

        [Test]
        public void ExtractStringWithMultiline()
        {
            const string input = "test1\r\ntest2\r\ntest3";

            var result = new TestResult();
            var extractor = new ColumnExtractor<TestResult, string>(0, CultureInfo.CurrentCulture);
            extractor.SetInto(a => a.Member1);

            extractor.Extract(result, input, out result);
            result.Member1.Should().Be(input);
        }

        [Test]
        [TestCase("1,1","fr-FR", 1.1)]
        [TestCase("1.1","en-GB", 1.1)]
        public void ExtractDecimalDependingCultureInfo(string input, string culture, decimal expected)
        {
            var result = new TestResult();
            var extractor = new ColumnExtractor<TestResult, decimal?>(0, new CultureInfo(culture));
            extractor.SetInto(a=>a.Member4);

            extractor.Extract(result, input, out result);
            result.Member4.Should().Be(expected);
        }

        [Test]
        [TestCase("01/07/1980", "fr-FR", 1980, 7, 1)]
        [TestCase("07/01/1980", "en-US", 1980, 7, 1)]
        public void ExtractDateTimeDependingCultureInfo(string input, string culture, int year, int month, int day)
        {
            var result = new TestResult();
            var extractor = new ColumnExtractor<TestResult, DateTime>(0, new CultureInfo(culture));
            extractor.SetInto(a => a.Member3);

            extractor.Extract(result, input, out result);
            result.Member3.Should().Be(new DateTime(year, month, day));
        }*/
    }
}