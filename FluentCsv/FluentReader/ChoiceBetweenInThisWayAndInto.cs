using System;
using System.Linq.Expressions;
using FluentCsv.CsvParser;

namespace FluentCsv.FluentReader
{
    public class ChoiceBetweenInThisWayAndInto<TLine, TMember> : ParserContainer<TLine> where TLine : new()
    {
        private readonly InThisWhayBuilder<TLine, TMember> _inThisWhayBuilder;
        private readonly IntoBuilder<TLine, TMember> _intoBuilder;

        public IntoBuilder<TLine, TMember> InThisWay(Func<string, TMember> factory)
        {
            return _inThisWhayBuilder.InThisWay(factory);
        }

        public ChoiceBetweenPutOrGetAll<TLine> Into(Expression<Func<TLine, TMember>> intoMember)
        {
            return _intoBuilder.Into(intoMember);
        }

        internal ChoiceBetweenInThisWayAndInto(CsvFileParser<TLine> parser, int columnIndex) : base(parser)
        {
            _inThisWhayBuilder = new InThisWhayBuilder<TLine, TMember>(CsvFileParser, columnIndex);
            _intoBuilder = new IntoBuilder<TLine, TMember>(CsvFileParser, columnIndex);
        }
    }
}