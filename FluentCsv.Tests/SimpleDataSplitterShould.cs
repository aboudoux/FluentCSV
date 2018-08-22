using FluentAssertions;
using FluentCsv.CsvParser.Splitters;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    public class SimpleDataSplitterShould
    {
        [Test]
        [TestCase("\r\n", "Column1;Column2", "Column1;Column2")]
        [TestCase("\r\n", "Column1;Column2\r\ntest;test", "Column1;Column2")]
        [TestCase("<endl>", "Column1;Column2<endl>test;test", "Column1;Column2")]
        [TestCase("\r\n", "", "")]
        public void ExtractOnlyFirstLine(string delimiter, string input, string expected)
        {
            var splitter = new SimpleDataSplitter();
            var firstLine = splitter.GetFirstLine(input, delimiter);
            firstLine.Should().Be(expected);
        }

	    [Test]
	    public void RemoveIfStringContainsBom() {
		    const string input = "\uFEFFTEST1,TEST2,TEST3\r\nA,B,C";

		    var splitter = new SimpleDataSplitter();
		    var firstLine = splitter.GetFirstLine(input, "\r\n");
		    var columns = splitter.SplitColumns(firstLine, ",");

		    columns.Should().HaveCount(3);
		    columns[0].Should().Be("TEST1");
	    }
	}
}