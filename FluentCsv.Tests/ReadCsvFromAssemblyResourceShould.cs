using System;
using FluentAssertions;
using FluentCsv.Exceptions;
using FluentCsv.FluentReader;
using FluentCsv.Tests.Results;
using Xunit;

namespace FluentCsv.Tests
{
    public class ReadCsvFromAssemblyResourceShould
    {
        [Fact]
        public void LoadResource1Csv()
        {
            var csv = Read.Csv.FromAssemblyResource("FluentCsv.Tests.CsvFiles.resource1.csv")
                .With.CultureInfo("fr-FR")
                .ThatReturns.ArrayOf<TestResult>()
                .Put.Column("name").Into(a => a.Member1)
                .Put.Column("date").As<DateTime>().Into(a => a.Member3)
                .GetAll();

            csv.ResultSet.ShouldContainEquivalentTo(
                TestResult.Create("TEST1", member3: new DateTime(2001,01,01)),
                TestResult.Create("TEST2", member3: new DateTime(2009,08,02))
                );
        }

        [Fact]
        public void ThrowErrorIfResourceNotFound()
        {
            Action action = () => Read.Csv.FromAssemblyResource("unknown")
                .ThatReturns.ArrayOf<TestResult>()
                .Put.Column("name").Into(a => a.Member1)
                .GetAll();

            action.Should().Throw<AssemblyResourceNotFoundException>();
        }
    }
}