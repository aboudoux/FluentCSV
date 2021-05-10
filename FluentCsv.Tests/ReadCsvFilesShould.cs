using System;
using System.Linq;
using System.Text;
using FluentAssertions;
using FluentCsv.FluentReader;
using Xunit;

namespace FluentCsv.Tests
{
    public class ReadCsvFilesShould
    {
        [Fact]
        public void LoadAnnuaireDebitTabac()
        {
            var sourceFile = GetTestFilePath
                .FromDirectory("CsvFiles")
                .AndFileName("annuaire-des-debits-de-tabac.csv");

            var result = Read.Csv.FromFile(sourceFile)
                .ThatReturns.ArrayOf<AnnuaireDebitTabacResult>()
                .Put.Column("ID").As<int>().Into(a => a.Id)
                .Put.Column("ENSEIGNE").Into(a => a.Enseigne)
                .Put.Column("NUMERO ET LIBELLE DE VOIE").Into(a => a.NumeroEtLibelle)
                .Put.Column("COMPLEMENT").Into(a => a.Complement)
                .Put.Column("CODE POSTAL").Into(a => a.CodePostal)
                .Put.Column("COMMUNE").Into(a => a.Commune)
                .GetAll();

            result.ResultSet.Should().HaveCount(25773);
            var testLine = result.ResultSet.First(a => a.Id == 566);

            testLine.Id.Should().Be(566);
            testLine.Enseigne.Should().Be("\"chez Clement\"");
            testLine.NumeroEtLibelle.Should().Be("29 cours de la republique");
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void LoadAnnuaireDebitTabacWithError()
        {
            var sourceFile = GetTestFilePath
                .FromDirectory("CsvFiles")
                .AndFileName("annuaire-des-debits-de-tabac.csv");

            var result = Read.Csv.FromFile(sourceFile)
                .ThatReturns.ArrayOf<AnnuaireDebitTabacResult>()
                .Put.Column("ID").As<int>().Into(a => a.Id)
                .Put.Column("ENSEIGNE").InThisWay(CheckIfTabac).Into(a => a.Enseigne)
                .Put.Column("NUMERO ET LIBELLE DE VOIE").Into(a => a.NumeroEtLibelle)
                .Put.Column("COMPLEMENT").Into(a => a.Complement)
                .Put.Column("CODE POSTAL").Into(a => a.CodePostal)
                .Put.Column("COMMUNE").Into(a => a.Commune)
                .GetAll();

            result.Errors.Should().HaveCount(14510);

            string CheckIfTabac(string enseigne)
            {
                if(enseigne != "Tabac")
                    throw new Exception("Not a tabac");
                return enseigne;
            }
        }

        [Fact]
        public void LoadAnnuaireEcolesDoctorales()
        {
            var sourceFile = GetTestFilePath
                .FromDirectory("CsvFiles")
                .AndFileName("fr-esr-ecoles_doctorales_annuaire.csv");

            var result = Read.Csv.EncodedIn(Encoding.UTF8).FromFile(sourceFile)
                .ThatReturns.ArrayOf<AnnuaireEcolesDoctoralesResult>()
                .Put.Column("adresse_postale").Into(a => a.AddressePostale)
                .Put.Column("code_etablissement_support").Into(a=>a.CodeEtablissementSupport)
                .GetAll();

            result.Errors.Should().BeEmpty();
            result.ResultSet.Should().HaveCount(267);
            var firstRow = result.ResultSet.First();

            firstRow.AddressePostale.Should().Be("Aix- Marseille Université\r\nFaculté des Sciences de Luminy\r\nCase 901\r\n163, avenue de Luminy");
        }

        [Fact]
        public void LoadAnnuaireDebitTabacWithTuple() {
	        var sourceFile = GetTestFilePath
		        .FromDirectory("CsvFiles")
		        .AndFileName("annuaire-des-debits-de-tabac.csv");

	        var result = Read.Csv.FromFile(sourceFile)
		        .ThatReturns.ArrayOf<(int Id,string Enseigne, string Cp)>()
		        .Put.Column("ID").As<int>().Into(a => a.Id)
		        .Put.Column("ENSEIGNE").MakingSureThat(IsTabac).Into(a => a.Enseigne)
		        .Put.Column("CODE POSTAL").Into(a => a.Cp)
		        .GetAll();

	        result.Errors.Should().HaveCount(14510);
            result.ResultSet.Should().HaveCount(11263);

            var testLine = result.ResultSet.First(a => a.Id == 565);

            testLine.Id.Should().Be(565);
	        testLine.Enseigne.Should().Be("Tabac");
	        testLine.Cp.Should().Be("03800");

	        Data IsTabac(string arg) 
		        => arg == "Tabac"
			        ? Data.Valid
			        : Data.Invalid("is not a tabac");
        }
    }

    public class AnnuaireDebitTabacResult
    {
        public int Id { get; set; }
        public string Enseigne { get; set; }
        public string NumeroEtLibelle { get; set; }
        public string Complement { get; set; }
        public string CodePostal { get; set; }
        public string Commune { get; set; }
    }

    public class AnnuaireEcolesDoctoralesResult
    {
        public string AddressePostale { get; set; }

        public string CodeEtablissementSupport { get; set; }
    }
}