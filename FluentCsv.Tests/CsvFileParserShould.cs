using FluentAssertions;
using FluentCsv.CsvParser;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    public class CsvFileParserShould
    {
        [Test]
        public void ParseSimpleCsvFromString()
        {
            var input = @"test1
Test2
test3";

            var parser = new CsvFileParser<TestResult>(input);
            parser.AddColumn(0, a=>a.Member1);
            var result = parser.Parse();
            result.Should().HaveCount(3);            
        }

        [Test]
        public void ParseCsvWithMultipleColumns()
        {
            var input = @"test1;1
test2;2
test3;3";

            var parser = new CsvFileParser<TestResult>(input);
            parser.AddColumn(0, a => a.Member1);
            parser.AddColumn(1, a => a.Member2);
            var result = parser.Parse();

            result.Should().HaveCount(3);

            result[0].Member1.Should().Be("test1");
            result[0].Member2.Should().Be(1);

            result[1].Member1.Should().Be("test2");
            result[1].Member2.Should().Be(2);

            result[2].Member1.Should().Be("test3");
            result[2].Member2.Should().Be(3);
        }
    }
}