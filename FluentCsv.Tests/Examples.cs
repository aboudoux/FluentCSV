using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using FluentCsv.FluentReader;
using Xunit;

namespace FluentCsv.Tests
{
    public class Examples
    {
        private const string CsvFiles = "CsvFiles";

        [Fact]
        public void Sample1()
        {
            var file = GetTestFilePath.FromDirectory(CsvFiles)
                .AndFileName("Sample1.csv");

            var csv = Read.Csv.FromFile(file)
	            .ThatReturns.ArrayOf<(string Name, int Age)>()
	            .Put.Column("name").Into(a => a.Name)
	            .Put.Column("age").As<int>().Into(a => a.Age)
	            .GetAll();

            Debug.WriteLine("CSV DATA");
            csv.ResultSet.ForEach(r=> Debug.WriteLine($"Name : {r.Name} - Age : {r.Age}"));

            Debug.WriteLine("ERRORS");
            csv.Errors.ForEach(e => Debug.WriteLine($"Error at line {e.LineNumber} column index {e.ColumnZeroBasedIndex} : {e.ErrorMessage}"));
        }

        [Fact]
        public void ExampleA()
        {
            var file = GetTestFilePath.FromDirectory(CsvFiles)
                .AndFileName("ExampleA.csv");

            var csv = Read.Csv.FromFile(file)
                .With.CultureInfo("en-US")
                .ThatReturns.ArrayOf<CsvInfos>()
                .Put.Column("FirstName").Into(p => p.Contact.Firstname)
                .Put.Column("LastName").Into(p => p.Contact.Lastname)
                .Put.Column("BirthDate").As<DateTime>().Into(p => p.Contact.BirthDate)
                .Put.Column("Street").Into(p => p.Address.Street)
                .Put.Column("City").Into(p => p.Address.City)
                .Put.Column("ZipCode").Into(p => p.Address.ZipCode)
                .GetAll();

            Console.WriteLine("CSV DATA");
            csv.ResultSet.ForEach(Console.WriteLine);
        }

        [Fact]
        public void ExampleB()
        {
            const string input = "NAME-name#Smith-Bob#Rob\"in-Wiliam";

            var csv = Read.Csv.FromString(input)
	            .With.EndOfLineDelimiter("#")
	            .And.ColumnsDelimiter("-")
	            .And.Header(As.CaseSensitive)
	            .And.SimpleParsingMode()
	            .ThatReturns.ArrayOf<(string FirstName, string LastName)>()
	            .Put.Column("NAME").Into(a => a.FirstName)
	            .Put.Column("name").Into(a => a.LastName)
	            .GetAll();

            Debug.WriteLine("CSV DATA");
            csv.ResultSet.ForEach(a=> Debug.WriteLine($"{a.FirstName} {a.LastName}"));
        }

        [Fact]
        public void ExampleC()
        {
            var file = GetTestFilePath.FromDirectory(CsvFiles)
                .AndFileName("ExampleC.csv");

            var csv = Read.Csv.FromFile(file)
                .With.ColumnsDelimiter("\t")
                .ThatReturns.ArrayOf<LineExampleC>()
                .Put.Column(0).As<int>().Into(a => a.Id)
                .Put.Column(1).As<PhoneNumber>().InThisWay(s => new PhoneNumber(s)).Into(a => a.PhoneNumber)
                .Put.Column(2).As<CustomEnum>().InThisWay(s => new CustomEnum(s)).Into(a => a.CustomEnum)
                .Put.Column(3).Into(a => a.Address)
                .GetAll();

            Console.WriteLine("CSV DATA");
            csv.ResultSet.ForEach(Console.WriteLine);

            Console.WriteLine("ERRORS");
            csv.Errors.ForEach(e => Console.WriteLine($"Error at line {e.LineNumber} column index {e.ColumnZeroBasedIndex} : {e.ErrorMessage}"));
        }

        [Fact]
        public void ExampleC1() {
	        var file = GetTestFilePath.FromDirectory(CsvFiles)
		        .AndFileName("ExampleC.csv");

	        var csv = Read.Csv.FromFile(file)
		        .With.ColumnsDelimiter("\t")
		        .ThatReturns.ArrayOf<(int Id, string PhoneNumber, string CustomEnum, string Address)>()
		        .Put.Column(0).As<int>().Into(a => a.Id)
		        .Put.Column(1).MakingSureThat(PhoneNumberIsValid).Into(a => a.PhoneNumber)
		        .Put.Column(2).MakingSureThat(EnumIsValid).InThisWay(a=>a.Replace("|"," and ")).Into(a => a.CustomEnum)
		        .Put.Column(3).Into(a => a.Address)
		        .GetAll();

	        Debug.WriteLine("CSV DATA");
	        csv.ResultSet.ForEach(a=>Debug.WriteLine($"id = {a.Id}, Phone = {a.PhoneNumber}, Enum = {a.CustomEnum}, Address = {a.Address}"));

	        Debug.WriteLine("ERRORS");
	        csv.Errors.ForEach(e => Debug.WriteLine($"Error at line {e.LineNumber} column index {e.ColumnZeroBasedIndex} : {e.ErrorMessage}"));

            Data PhoneNumberIsValid(string phone)
                =>  Regex.IsMatch(phone, "[0-9]{10}")
                ? Data.Valid 
                : Data.Invalid("Phone number is invalid");

            Data EnumIsValid(string @enum)
            => @enum.Split('|').ToList().TrueForAll(a => a == "A" || a == "B" || a == "F")
		            ? Data.Valid
		            : Data.Invalid("Invalid enum character");
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

        public override string ToString() => $"{Contact} - {Address}";
    }

    public class Contact
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime BirthDate { get; set; }

        public override string ToString() => $"{Firstname} {Lastname} ({BirthDate:d})";
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }

        public override string ToString() => $"City : {City}";
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