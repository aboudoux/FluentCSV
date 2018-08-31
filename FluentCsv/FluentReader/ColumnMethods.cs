using System;
using System.Linq.Expressions;
using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Results;

namespace FluentCsv.FluentReader
{
    public class ChoiceBetweenAsAndInto<TLine, TResultSet> : ParserContainer<TLine, TResultSet> 
        where TLine : new()
        where TResultSet : class, ICsvResult<TLine>
    {
        private readonly IntoBuilder<TLine, string, TResultSet> _intoBuilder;

        private readonly AsBuilder<TLine, TResultSet> _asBuilder;

        private readonly InThisWhayBuilder<TLine, string, TResultSet> _inThisWhayBuilder;

        internal ChoiceBetweenAsAndInto(CsvFileParser<TLine> parser, int columnIndex, TResultSet resultset) : base(parser, resultset)
        {
            _intoBuilder = new IntoBuilder<TLine, string, TResultSet>(CsvFileParser, columnIndex, resultset);
            _asBuilder = new AsBuilder<TLine, TResultSet>(CsvFileParser, columnIndex, resultset);
            _inThisWhayBuilder = new InThisWhayBuilder<TLine, string, TResultSet>(CsvFileParser, columnIndex, resultset);
        }

        internal ChoiceBetweenAsAndInto(CsvFileParser<TLine> parser, string columnName, TResultSet resultSet) : base(parser, resultSet)
        {
            _intoBuilder = new IntoBuilder<TLine, string, TResultSet>(CsvFileParser, columnName, resultSet);
            _asBuilder = new AsBuilder<TLine, TResultSet>(CsvFileParser, columnName, resultSet);
            _inThisWhayBuilder = new InThisWhayBuilder<TLine, string, TResultSet>(CsvFileParser, columnName, resultSet);
        }

        public AsMethods<TLine, T, TResultSet> As<T>()
        {
            return _asBuilder.As<T>();
        }

        public IntoConstraints<TLine, TResultSet> Into(Expression<Func<TLine, string>> intoMember)
        {
            return _intoBuilder.Into(intoMember);
        }

        public IntoBuilder<TLine, string, TResultSet> InThisWay(Func<string, string> inThisWay)
        {
            return _inThisWhayBuilder.InThisWay(inThisWay);
        }
    }
}