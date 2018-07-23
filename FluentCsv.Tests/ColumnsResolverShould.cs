using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using FluentAssertions;
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
    }

    public class ColumnsResolver<TResult> where TResult : new()
    {
        private readonly Dictionary<int, IColumnExtractor> _columns = new Dictionary<int, IColumnExtractor>();

        public void AddColumn<TMember>(int index, Expression<Func<TResult, TMember>> into, Func<string, TMember> setInThisWay = null)
        {
            var extractor = new ColumnExtractor<TResult, TMember>();
            extractor.SetInto(into);

            if(setInThisWay != null)
                extractor.SetInThisWay(setInThisWay);
            
            _columns.Add(index, extractor);
        }

        public TResult GetResult(string[] rawColumns)
        {
            var result = new TResult();
            foreach (var extractor in _columns)
                extractor.Value.Extract(result, rawColumns[extractor.Key]);
            return result;
        }
    }
}