using FluentAssertions;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    public class ReadCsvFromStringShould
    {
        [Test]
        public void ReturnsStringsForCsvWithOneColumn()
        {
            var csv = @"test1
test2
test3";

            var result = Read
                .Csv
                .FromString(csv)
                .That.ReturnsLinesOf<string>()
                .Put.Column(0).Into(a => a)
                .GetAll();

            result.Should().HaveCount(3);
        }
    }
}