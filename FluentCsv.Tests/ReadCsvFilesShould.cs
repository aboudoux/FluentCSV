using System;
using System.Linq;
using System.Text;
using FluentAssertions;
using FluentCsv.FluentReader;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    public class ReadCsvFilesShould
    {
        [Test]
        public void LoadAnnuaireDebitTabac()
        {
            var sourceFile = GetTestFilePath
                .FromDirectory("CsvFiles")
                .AndFileName("annuaire-des-debits-de-tabac.csv");

            var result = Read.Csv.FromFile(sourceFile)
                .ThatReturns.LinesOf<AnnuaireDebitTabacResult>()
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

        [Test]
        public void LoadAnnuaireDebitTabacWithError()
        {
            var sourceFile = GetTestFilePath
                .FromDirectory("CsvFiles")
                .AndFileName("annuaire-des-debits-de-tabac.csv");

            var result = Read.Csv.FromFile(sourceFile)
                .ThatReturns.LinesOf<AnnuaireDebitTabacResult>()
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

        [Test]
        public void LoadAnnuaireEcolesDoctorales()
        {
            var sourceFile = GetTestFilePath
                .FromDirectory("CsvFiles")
                .AndFileName("fr-esr-ecoles_doctorales_annuaire.csv");

            var result = Read.Csv.FromFile(sourceFile, Encoding.UTF8)
                .ThatReturns.LinesOf<AnnuaireEcolesDoctoralesResult>()
                .Put.Column("adresse_postale").Into(a => a.AddressePostale)
                .Put.Column("code_etablissement_support").Into(a=>a.CodeEtablissementSupport)
                .GetAll();

            result.Errors.Should().BeEmpty();
            result.ResultSet.Should().HaveCount(267);
            var firstRow = result.ResultSet.First();

            firstRow.AddressePostale.Should().Be("Aix- Marseille Université\nFaculté des Sciences de Luminy\nCase 901\n163, avenue de Luminy");
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