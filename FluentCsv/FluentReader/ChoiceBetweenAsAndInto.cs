using System;
using System.Linq.Expressions;
using FluentCsv.CsvParser;

namespace FluentCsv.FluentReader
{
    public class ChoiceBetweenAsAndInto<TLine> : ParserContainer<TLine> where TLine : new()
    {
        private readonly IntoBuilder<TLine, string> _intoBuilder;
        private readonly AsBuilder<TLine> _asBuilder;

        public ChoiceBetweenInThisWayAndInto<TLine, T> As<T>()
        {
            return _asBuilder.As<T>();
        }

        public ChoiceBetweenPutOrGetAll<TLine> Into(Expression<Func<TLine, string>> intoMember)
        {
            return _intoBuilder.Into(intoMember);
        }

        internal ChoiceBetweenAsAndInto(CsvFileParser<TLine> parser, int columnIndex) : base(parser)
        {
            _intoBuilder = new IntoBuilder<TLine, string>(CsvFileParser, columnIndex);
            _asBuilder = new AsBuilder<TLine>(CsvFileParser, columnIndex);
        }
    }
}