using System.Linq;
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
                .That.ReturnsLinesOf<AnnuaireDebitTabacResult>()
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
}