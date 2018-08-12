using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using FluentCsv.FluentReader;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    public class Examples
    {
        private const string CsvFiles = "CsvFiles";

        [Test]
        public void Sample1()
        {
            var file = GetTestFilePath.FromDirectory(CsvFiles)
                .AndFileName("Sample1.csv");

            var csvData = Read.Csv.FromFile(file)
                .ThatReturns.LinesOf<CsvData>()
                .Put.Column("name").Into(a => a.Name)
                .Put.Column("age").As<int>().Into(a => a.Age)
                .GetAll();

            Console.WriteLine("CSV DATA");
            csvData.ResultSet.ForEach(r=> Console.WriteLine($"Name : {r.Name} - Age : {r.Age}"));

            Console.WriteLine("ERRORS");
            csvData.Errors.ForEach(e => Console.WriteLine($"Error at line {e.LineNumber} column index {e.ColumnZeroBasedIndex} : {e.ErrorMessage}"));
        }

        [Test]
        public void ExampleA()
        {
            var file = GetTestFilePath.FromDirectory(CsvFiles)
                .AndFileName("ExampleA.csv");

            var csvData = Read.Csv.FromFile(file)
                .ThatReturns.LinesOf<CsvInfos>()
                .Put.Column("FirstName").Into(p => p.Contact.Firstname)
                .Put.Column("LastName").Into(p => p.Contact.Lastname)
                .Put.Column("BirthDate").As<DateTime>()
                .InThisWay(a => DateTime.ParseExact(a, "MMddyy", CultureInfo.CurrentCulture))
                .Into(p => p.Contact.BirthDate)
                .Put.Column("Address").Into(p => p.Address.Street)
                .Put.Column("City").Into(p => p.Address.City)
                .Put.Column("ZipCode").Into(p => p.Address.ZipCode)
                .GetAll();

        }

        [Test]
        public void ExampleC()
        {
            var file = GetTestFilePath.FromDirectory(CsvFiles)
                .AndFileName("ExampleC.csv");

            var csvData = Read.Csv.FromFile(file)
                .With.ColumnsDelimiter("\t")
                .ThatReturns.LinesOf<LineExampleC>()
                .Put.Column(0).As<int>().Into(a => a.Id)
                .Put.Column(1).As<PhoneNumber>().InThisWay(s => new PhoneNumber(s)).Into(a => a.PhoneNumber)
                .Put.Column(2).As<CustomEnum>().InThisWay(s => new CustomEnum(s)).Into(a => a.CustomEnum)
                .Put.Column(3).Into(a => a.Address)
                .GetAll();

            Console.WriteLine("CSV DATA");
            csvData.ResultSet.ForEach(Console.WriteLine);

            Console.WriteLine("ERRORS");
            csvData.Errors.ForEach(e => Console.WriteLine($"Error at line {e.LineNumber} column index {e.ColumnZeroBasedIndex} : {e.ErrorMessage}"));

        }
    }

    public class CsvData
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class CsvInfos
    {
        public Contact Contact { get; } = new Contact();
        public Address Address { get; } = new Address();
    }

    public class Contact
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime BirthDate { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }

    public class LineExampleC
    {
        public int Id { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public CustomEnum CustomEnum { get; set; }
        public string Address { get; set; }

        public override string ToString()
            => $"id = {Id}, Phone = {PhoneNumber}, Enum = {CustomEnum}, Address = {Address}";
    }

    public class PhoneNumber
    {
        private readonly string _phoneNumber;

        public PhoneNumber(string phone)
        {
            if(!Regex.IsMatch(phone,"[0-9]{10}"))
                throw new ArgumentException("Phone number is invalid");

            _phoneNumber = phone;
        }

        public override string ToString()
            => _phoneNumber;
    }

    public class CustomEnum
    {
        private List<string> _enumList;

        public CustomEnum(string @enum)
        {
            _enumList = @enum.Split('|').ToList();
            if(!_enumList.TrueForAll(a => a == "A" || a == "B" || a == "F"))
                throw new ArgumentException("Invalid enum character");
        }

        public override string ToString()
            => string.Join(" and ", _enumList);
    }
}