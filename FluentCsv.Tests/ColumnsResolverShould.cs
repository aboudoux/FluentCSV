using System;
using System.Globalization;
using FluentAssertions;
using FluentCsv.CsvParser;
using FluentCsv.Exceptions;
using FluentCsv.Tests.Results;
using Xunit;

namespace FluentCsv.Tests
{
    public class ColumnsResolverShould
    {
        private CultureInfo TestCulture = new CultureInfo("fr-FR");

        [Fact]
        public void ExtractSomeColumns()
        {
            var resolver = new ColumnsResolver<TestResult>(TestCulture);
            resolver.AddColumn(0, a=>a.Member1);
            resolver.AddColumn(1, a=>a.Member2);
            resolver.AddColumn(2, a=>a.Member3, a => DateTime.ParseExact(a,"ddMMyyyy", CultureInfo.CurrentCulture));
            var result = resolver.GetResult(new[] {"coucou", "5", "01071980"}, 1);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(TestResult.Create("coucou",5, new DateTime(1980, 07, 01)));
        }

        [Fact]
        public void ThrowErrorIfAddColumnWithSameIndex()
        {
            var resolver = new ColumnsResolver<TestResult>(TestCulture);
            resolver.AddColumn(0, a=>a.Member1);
            Action error = () => resolver.AddColumn(0, a => a.Member1);
            error.Should().Throw<ColumnWithSameIndexAlreadyDeclaredException>();
        }

        [Fact]
        public void IncludeDeeperProperties()
        {
            var resolver = new ColumnsResolver<DeepResult>(TestCulture);
            resolver.AddColumn(0, a=>a.Contact.Firstname);

            var result = resolver.GetResult(new[] {"MARTIN"}, 1) as DeepResult;

            result.Should().NotBeNull();
            result.Contact.Firstname.Should().Be("MARTIN");
        }

        [Fact]
        public void IncludeDeeperPropertiesWithMoreLevel()
        {
            var resolver = new ColumnsResolver<DeepResult>(TestCulture);
            resolver.AddColumn(0, a => a.Contact.Infos.Comment);

            var result = resolver.GetResult(new[] { "MARTIN" }, 1) as DeepResult;

            result.Should().NotBeNull();
            result.Contact.Infos.Comment.Should().Be("MARTIN");
        }

        [Fact]
        public void ThrowErrorIfDeeperPropertiesIsNullInstance()
        {
            var resolver = new ColumnsResolver<DeepResult>(TestCulture);
            resolver.AddColumn(0, a => a.Address.City);

            Action action = () => resolver.GetResult(new[] {"MARTIN"}, 1);

            action.Should().Throw<NullPropertyInstanceException>()
                .Which.Message.Should().Be("A property of type class or struct of your resultset is null. Please, set all your subclass in your resultset with the new keyword. PropertyName : Address - PropertyType : FluentCsv.Tests.Results.AddressResult");
        }
    }
}