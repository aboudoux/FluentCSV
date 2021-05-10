using System;
using System.Globalization;
using FluentAssertions;
using FluentCsv.CsvParser;
using FluentCsv.Tests.Results;
using Xunit;

namespace FluentCsv.Tests
{
    public class ColumnExtractorShould
    {
        [Fact]
        public void ExtractStringAndPutInField()
        {
            var source = new TestResult();
            var result = source as object;
            var extractor = new ColumnExtractor<TestResult, string>(0, CultureInfo.CurrentCulture);
            extractor.SetInto(a=>a.Member1);

            extractor.Extract(source,"bonjour", out result);
            source.Member1.Should().Be("bonjour");
        }

        
        [Fact]
        public void ExtractIntAndPutInField()
        {
	        var source = new TestResult();
	        var result = source as object;
            var extractor = new ColumnExtractor<TestResult, int>(0, CultureInfo.CurrentCulture);
            extractor.SetInto(a => a.Member2);

            extractor.Extract(result, "25", out result);
            source.Member2.Should().Be(25);
        }
        
        [Fact]
        public void ExtractDateAndPutInField()
        {
	        var source = new TestResult();
	        var result = source as object;
            var extractor = new ColumnExtractor<TestResult, DateTime>(0, CultureInfo.CurrentCulture);
            extractor.SetInto(a => a.Member3);

            extractor.Extract(result, "01/07/1980", out result);
            source.Member3.Should().Be(new DateTime(1980,7,1));
        }
        
        [Fact]
        public void ExtractObjectInSpecialWay()
        {
	        var source = new TestResult();
	        var result = source as object;
            var extractor = new ColumnExtractor<TestResult, string>(0, CultureInfo.CurrentCulture);
            extractor.SetInto(a => a.Member1);
            extractor.SetInThisWay(a=>$"[TEST]{a}");

            extractor.Extract(result, "ESSAI", out result);
            source.Member1.Should().Be("[TEST]ESSAI");
        }

        [Fact]
        public void ExtractStringWithMultiline()
        {
            const string input = "test1\r\ntest2\r\ntest3";

            var source = new TestResult();
            var result = source as object;
            var extractor = new ColumnExtractor<TestResult, string>(0, CultureInfo.CurrentCulture);
            extractor.SetInto(a => a.Member1);

            extractor.Extract(result, input, out result);
            source.Member1.Should().Be(input);
        }

        [Theory]
        [InlineData("1,1","fr-FR", 1.1)]
        [InlineData("1.1","en-GB", 1.1)]
        public void ExtractDecimalDependingCultureInfo(string input, string culture, decimal expected)
        {
	        var source = new TestResult();
	        var result = source as object;
            var extractor = new ColumnExtractor<TestResult, decimal?>(0, new CultureInfo(culture));
            extractor.SetInto(a=>a.Member4);

            extractor.Extract(result, input, out result);
            source.Member4.Should().Be(expected);
        }

        [Theory]
        [InlineData("01/07/1980", "fr-FR", 1980, 7, 1)]
        [InlineData("07/01/1980", "en-US", 1980, 7, 1)]
        public void ExtractDateTimeDependingCultureInfo(string input, string culture, int year, int month, int day)
        {
	        var source = new TestResult();
	        var result = source as object;
            var extractor = new ColumnExtractor<TestResult, DateTime>(0, new CultureInfo(culture));
            extractor.SetInto(a => a.Member3);

            extractor.Extract(result, input, out result);
            source.Member3.Should().Be(new DateTime(year, month, day));
        }
    }
}