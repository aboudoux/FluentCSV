using System;
using System.Linq.Expressions;
using FluentCsv.CsvParser;

namespace FluentCsv.FluentReader
{
    public class IntoBuilder<TLine, TMember> : ParserContainer<TLine> where TLine : new()
    {
        private readonly int _columnIndex;
        private readonly string _columnName;
        private readonly Func<string, TMember> _inThisWay;

        internal IntoBuilder(CsvFileParser<TLine> parser, int columnIndex, Func<string, TMember> inThisWay = null) : base(parser)
        {
            _columnIndex = columnIndex;
            _inThisWay = inThisWay;
        }

        internal IntoBuilder(CsvFileParser<TLine> parser, string columnName, Func<string, TMember> inThisWay = null) : base(parser)
        {
            _columnName = columnName;
            _inThisWay = inThisWay;
        }

        public ChoiceBetweenPutOrGetAll<TLine> Into(Expression<Func<TLine, TMember>> intoMember)
        {
            if(_columnName.IsEmpty())
                CsvFileParser.AddColumn(_columnIndex, intoMember, _inThisWay);
            else
                CsvFileParser.AddColumn(_columnName, intoMember, _inThisWay);

            return new ChoiceBetweenPutOrGetAll<TLine>(CsvFileParser);
        }
    }
}