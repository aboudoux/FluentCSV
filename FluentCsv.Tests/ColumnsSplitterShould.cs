using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Splitters;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    public class ColumnsSplitterShould
    {
        [Test]
        [TestCase(";", "Aurelien;BOUDOUX;\"9\r\nrue du test; impasse\r\n75001\r\nParis\"", "Aurelien", "BOUDOUX", "9\r\nrue du test; impasse\r\n75001\r\nParis")]
        [TestCase("<->", "Aurelien<->BOUDOUX<->\"9\r\nrue du test; <-> impasse <->\r\n75001\r\nParis\"", "Aurelien", "BOUDOUX", "9\r\nrue du test; <-> impasse <->\r\n75001\r\nParis")]        
        [TestCase(",", "\"M. Aurelien BOUDOUX, TEST\",,", "M. Aurelien BOUDOUX, TEST","","")]
        [TestCase(",", "\"TEST1\",\"TEST2\",\"TEST3\"","TEST1","TEST2","TEST3")]
        [TestCase(",", "TEST1,TEST2,TEST3","TEST1","TEST2","TEST3")]
        [TestCase(",", "TEST1,\"TEST2,TEST3\",TEST4","TEST1","TEST2,TEST3","TEST4")]
        [TestCase(",", "TEST1,\"TEST2,TEST3\",","TEST1","TEST2,TEST3","")]
        [TestCase(",", "\"Aurelien, \"\"BOUDOUX\"\"\",TEST,", "Aurelien, \"BOUDOUX\"", "TEST", "")]
        [TestCase(",", "\"\"\"\",\"\",\" \"\" \"", "\"", "", " \" ")]
        public void SplitColumnsInRfc4180(string delimiter, string input, string expected1, string expected2, string expected3)
        {
            var splitter = new Rfc4180ColumnSplitter();
            var result = splitter.Split(input, delimiter);

            result.Should().HaveCount(3);
            result[0].Should().Be(expected1);
            result[1].Should().Be(expected2);
            result[2].Should().Be(expected3);
        }
    }
}