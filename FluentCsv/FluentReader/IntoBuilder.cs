using System;
using System.Linq.Expressions;
using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Results;

namespace FluentCsv.FluentReader
{
    public class IntoBuilder<TLine, TMember, TResultSet> : ParserContainer<TLine, TResultSet> 
        where TLine : new()
        where TResultSet : class, ICsvResult<TLine>
    {
        private readonly int _columnIndex;
        private readonly string _columnName;
        private readonly Func<string, TMember> _inThisWay;

        internal IntoBuilder(CsvFileParser<TLine> parser, int columnIndex, TResultSet resultSet, Func<string, TMember> inThisWay = null) : base(parser, resultSet)
        {
            _columnIndex = columnIndex;
            _inThisWay = inThisWay;
        }

        internal IntoBuilder(CsvFileParser<TLine> parser, string columnName, TResultSet resultSet, Func<string, TMember> inThisWay = null) : base(parser, resultSet)
        {
            _columnName = columnName;
            _inThisWay = inThisWay;
        }

        public IntoConstraints<TLine, TResultSet> Into(Expression<Func<TLine, TMember>> intoMember)
        {
            if(_columnName.IsEmpty())
                CsvFileParser.AddColumn(_columnIndex, intoMember, _inThisWay);
            else
                CsvFileParser.AddColumn(_columnName, intoMember, _inThisWay);

            return new IntoConstraints<TLine, TResultSet>(CsvFileParser, ResultSet);
        }
    }
}