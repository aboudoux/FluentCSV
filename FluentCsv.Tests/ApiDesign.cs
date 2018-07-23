using System;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    public class ApiDesign
    {
        [Test]
        public void TestMethod1()
        {
            var data = Read
                .Csv
                .FromString("test")
                    .Where.LinesEndWith("\r\n")
                    .And.ColumnsAreDelimitedBy(";")
                .That.ReturnsLinesOf<CsvLine>()
                    .Put.Column(0).As<DateTime>().Into(a => a.P3)
                    .Put.Column(2).Into(a => a.P2)
                    .Put.Column(3).As<Address>().InThisWay(a=>new Address(a)).Into(a=>a.Address)
                .GetAll();

            Read2.Csv
                .FromString("test")
                .Where.ColumnsAreDelimitedBy("")
                .And.LineEndWith("")
                .That.ReturnsListOf<string>();


        }
    }

    public class CsvLine
    {
        public int P1 { get; set; }
        public string P2 { get; set; }
        public DateTime P3 { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public Address(string data)
        {
            
        }

        public string Rue { get; set; }
        public string Ville { get; set; }
    }
}
