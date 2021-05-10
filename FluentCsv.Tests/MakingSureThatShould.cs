using System;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentCsv.FluentReader;
using Xunit;

namespace FluentCsv.Tests
{
	public class MakingSureThatShould
	{
		[Fact]
		public void ReportSimpleError() 
		{
			var csv = Read.Csv.FromString("A;B\r\n1;3")
				.ThatReturns.ArrayOf<CsvData>()
				.Put.Column("A").MakingSureThat(Equals2).As<int>().Into(a => a.Age)
				.GetAll();

			csv.Errors.Length.Should().Be(1);
			csv.Errors.First().ErrorMessage.Should().Be("coucou");

			Data Equals2(string a)
				=> a == "2"
					? Data.Valid
					: Data.Invalid("coucou");
		}

		[Fact]
		public void BeFastOnLargeFile()
		{
			var sourceFile = GetTestFilePath
				.FromDirectory("CsvFiles")
				.AndFileName("annuaire-des-debits-de-tabac.csv");

			FluentActions.Invoking(()=>{
			var result = Read.Csv.FromFile(sourceFile)
				.ThatReturns.ArrayOf<AnnuaireDebitTabacResult>()
				.Put.Column("ID").As<int>().Into(a => a.Id)
				.Put.Column("ENSEIGNE").MakingSureThat(IsTabac).Into(a => a.Enseigne)
				.Put.Column("NUMERO ET LIBELLE DE VOIE").Into(a => a.NumeroEtLibelle)
				.Put.Column("COMPLEMENT").Into(a => a.Complement)
				.Put.Column("CODE POSTAL").Into(a => a.CodePostal)
				.Put.Column("COMMUNE").Into(a => a.Commune)
				.GetAll();

			Data IsTabac(string source) => source == "Tabac" ? Data.Valid : Data.Invalid("N'est pas un tabac");
			result.Errors.Count().Should().Be(14510);
			}).ExecutionTime().Should().BeLessOrEqualTo(TimeSpan.FromSeconds(3));
		}

		[Fact]
		public void WorkWithMultipleColumnsByName()
		{
			var result = Read.Csv.FromString("A;B\r\n1;2\r\n3;2")
				.ThatReturns.ArrayOf<CsvData>()
				.Put.Column("A").MakingSureThat(a => a == "3" ? Data.Valid : Data.Invalid("not")).Into(a => a.Name)
				.Put.Column("B").MakingSureThat(a => a == "3" ? Data.Valid : Data.Invalid("not")).As<int>()
				.Into(a => a.Age)
				.GetAll();

			result.Errors.Length.Should().Be(2);
		}

		[Fact]
		public void WorkWithMultipleColumnsByIndex() {
			var result = Read.Csv.FromString("A;B\r\n1;2\r\n3;2")
				.With.Header()
				.ThatReturns.ArrayOf<CsvData>()
				.Put.Column(0).MakingSureThat(a => a == "3" ? Data.Valid : Data.Invalid("not")).Into(a => a.Name)
				.Put.Column(1).MakingSureThat(a => a == "3" ? Data.Valid : Data.Invalid("not")).As<int>()
				.Into(a => a.Age)
				.GetAll();

			result.Errors.Length.Should().Be(2);
		}

		[Fact]
		public void WorkWithException()
		{
			var result = Read.Csv.FromString("Phone1;Phone2\r\n123456;232321\r\n0614894072;23432132")
				.ThatReturns.ArrayOf<TestCsvPhone>()
				.Put.Column("Phone1").MakingSureThat(ValidPhoneNumber).Into(a => a.Phone)
				.Put.Column("Phone2").As<PhoneNumber>().InThisWay(a=>new PhoneNumber(a)).Into(a=>a.PhoneNumber)
				.GetAll();

			Data ValidPhoneNumber(string value)
				=> Regex.IsMatch(value, "[0-9]{10}")
					? Data.Valid
					: Data.Invalid($"{value} is not a valid phone number");

			result.Errors.Length.Should().Be(2);
			result.Errors.ElementAt(0).ErrorMessage.Should().Be("123456 is not a valid phone number");
			result.Errors.ElementAt(1).ErrorMessage.Should().Be("Phone number is invalid");
		}

		public class TestCsvPhone
		{
			public string Phone { get; set; }
			public PhoneNumber PhoneNumber { get; set; }
		}
	}
}