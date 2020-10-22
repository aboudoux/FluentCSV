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
        private readonly InThisWayBuilder<TLine, TMember, TResultSet> _inThisWayBuilder;
        private readonly IntoBuilder<TLine, TMember, TResultSet> _intoBuilder;

        public IntoBuilder<TLine, TMember, TResultSet> InThisWay(Func<string, TMember> factory)
        {
            return _inThisWayBuilder.InThisWay(factory);
        }

        public IntoConstraints<TLine, TResultSet> Into(Expression<Func<TLine, TMember>> intoMember)
        {
            return _intoBuilder.Into(intoMember);
        }

        internal AsMethods(CsvFileParser<TLine> parser, int columnIndex, TResultSet resultSet, Func<string, Data> dataValidator = null) : base(parser, resultSet)
        {
            _inThisWayBuilder = new InThisWayBuilder<TLine, TMember, TResultSet>(CsvFileParser, columnIndex, resultSet, dataValidator);
            _intoBuilder = new IntoBuilder<TLine, TMember, TResultSet>(CsvFileParser, columnIndex, resultSet, validator:dataValidator);
        }
        internal AsMethods(CsvFileParser<TLine> parser, string columnName, TResultSet resultSet, Func<string, Data> dataValidator = null) : base(parser, resultSet)
        {
            _inThisWayBuilder = new InThisWayBuilder<TLine, TMember, TResultSet>(CsvFileParser, columnName, resultSet, dataValidator);
            _intoBuilder = new IntoBuilder<TLine, TMember, TResultSet>(CsvFileParser, columnName, resultSet, validator:dataValidator);
        }
    }
}