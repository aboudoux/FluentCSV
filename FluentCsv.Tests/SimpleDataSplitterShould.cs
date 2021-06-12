using System;
using FluentAssertions;
using FluentCsv.CsvParser.Splitters;
using Xunit;

namespace FluentCsv.Tests
{
    public class SimpleDataSplitterShould
    {
        [Theory]
        [InlineData("\r\n", "Column1;Column2", "Column1;Column2")]
        [InlineData("\r\n", "Column1;Column2\r\ntest;test", "Column1;Column2")]
        [InlineData("<endl>", "Column1;Column2<endl>test;test", "Column1;Column2")]
        [InlineData("\r\n", "", "")]
        public void ExtractOnlyFirstLine(string delimiter, string input, string expected)
        {
            var splitter = new SimpleDataSplitter();
            var firstLine = splitter.GetFirstLine(input, delimiter);
            firstLine.Should().Be(expected);
        }

        //[Theory]
        //[InlineData(";","Column1;Column2",2)]
        //public void SplitColumn(string delimiter, string input, int lenExpected)
        //{
	       // var splitter = new SimpleDataSplitter();
	       // var r = splitter.SplitColumns(input, delimiter);
	       // r.Length.Should().Be(lenExpected);
        //}

        public class SplitLinesData : TheoryData<string,string,string[]>
        {
	        public SplitLinesData()
	        {
		        Add("\r\n","Line1", new []{"Line1"});
		        Add("\r\n","Line1\r\nLine2", new []{"Line1","Line2"});
		        Add("\r\n","Line1\r\nLine2\r\n", new []{"Line1","Line2",""});
		        Add("\r\n","Line1\r\nLine2\r\nLine3", new []{"Line1","Line2","Line3"});
		        Add("---","A", new []{"A"});
		        Add("-","A-B-C-D", new []{"A","B","C","D"});
		        Add("--","--", Array.Empty<string>());
		        Add("\r\n","\r\n", Array.Empty<string>());
	        }
        }

        [Theory]
        [ClassData(typeof(SplitLinesData))]
        public void SplitLines(string delimiter, string input, string[] expected)
        {
	        var splitter = new SimpleDataSplitter();
	        var result = splitter.SplitLines(input, delimiter);
	        result.Should().BeEquivalentTo(expected);
        }
	}
}