using System;
using System.Globalization;
using FluentAssertions;
using FluentCsv.Exceptions;
using NUnit.Framework;

namespace FluentCsv.Tests
{
    public class ColumnsResolverShould
    {
        [Test]
        public void ExtractrSomeColumns()
        {
            var resolver = new ColumnsResolver<TestResult>();
            resolver.AddColumn(0, a=>a.Member1);
            resolver.AddColumn(1, a=>a.Member2);
            resolver.AddColumn(2, a=>a.Member3, a => DateTime.ParseExact(a,"ddMMyyyy", CultureInfo.CurrentCulture));
            var result = resolver.GetResult(new[] {"coucou", "5", "01071980"});

            result.Should().NotBeNull();
            result.Member1.Should().Be("coucou");
            result.Member2.Should().Be(5);
            result.Member3.Should().Be(new DateTime(1980,07,01));
        }

        [Test]
        public void ThrowErrorIfAddColumnWithSameIndex()
        {
            var resolver = new ColumnsResolver<TestResult>();
            resolver.AddColumn(0, a=>a.Member1);
            Action error = () => resolver.AddColumn(0, a => a.Member1);
            error.Should().Throw<ColumnWithSameIndexAlreadyDeclaredException>();
        }
    }
}