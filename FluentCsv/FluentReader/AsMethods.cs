using System;
using System.Linq.Expressions;
using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Results;

namespace FluentCsv.FluentReader
{
    public class AsMethods<TLine, TMember, TResultSet> : ParserContainer<TLine, TResultSet> 
        where TLine : new()
        where TResultSet : class , ICsvResult<TLine>
    {
        private readonly InThisWhayBuilder<TLine, TMember, TResultSet> _inThisWhayBuilder;
        private readonly IntoBuilder<TLine, TMember, TResultSet> _intoBuilder;

        public IntoBuilder<TLine, TMember, TResultSet> InThisWay(Func<string, TMember> factory)
        {
            return _inThisWhayBuilder.InThisWay(factory);
        }

        public IntoConstraints<TLine, TResultSet> Into(Expression<Func<TLine, TMember>> intoMember)
        {
            return _intoBuilder.Into(intoMember);
        }

        internal AsMethods(CsvFileParser<TLine> parser, int columnIndex, TResultSet resultSet) : base(parser, resultSet)
        {
            _inThisWhayBuilder = new InThisWhayBuilder<TLine, TMember, TResultSet>(CsvFileParser, columnIndex, resultSet);
            _intoBuilder = new IntoBuilder<TLine, TMember, TResultSet>(CsvFileParser, columnIndex, resultSet);
        }
        internal AsMethods(CsvFileParser<TLine> parser, string columnName, TResultSet resultset) : base(parser, resultset)
        {
            _inThisWhayBuilder = new InThisWhayBuilder<TLine, TMember, TResultSet>(CsvFileParser, columnName, resultset);
            _intoBuilder = new IntoBuilder<TLine, TMember, TResultSet>(CsvFileParser, columnName, resultset);
        }
    }
}