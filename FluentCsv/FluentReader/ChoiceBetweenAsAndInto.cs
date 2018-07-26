using System;
using System.Linq.Expressions;
using FluentCsv.CsvParser;

namespace FluentCsv.FluentReader
{
    public class ChoiceBetweenAsAndInto<TLine> : ParserContainer<TLine> where TLine : new()
    {
        private readonly IntoBuilder<TLine, string> _intoBuilder;
        private readonly AsBuilder<TLine> _asBuilder;
        private readonly InThisWhayBuilder<TLine, string> _inThisWhayBuilder;

        public ChoiceBetweenInThisWayAndInto<TLine, T> As<T>()
        {
            return _asBuilder.As<T>();
        }

        public ChoiceBetweenPutOrGetAll<TLine> Into(Expression<Func<TLine, string>> intoMember)
        {
            return _intoBuilder.Into(intoMember);
        }

        public IntoBuilder<TLine, string> InThisWay(Func<string, string> inThisWay)
        {
            return _inThisWhayBuilder.InThisWay(inThisWay);
        }

        internal ChoiceBetweenAsAndInto(CsvFileParser<TLine> parser, int columnIndex) : base(parser)
        {
            _intoBuilder = new IntoBuilder<TLine, string>(CsvFileParser, columnIndex);
            _asBuilder = new AsBuilder<TLine>(CsvFileParser, columnIndex);
            _inThisWhayBuilder = new InThisWhayBuilder<TLine, string>(CsvFileParser, columnIndex);
        }

        internal ChoiceBetweenAsAndInto(CsvFileParser<TLine> parser, string columnName) : base(parser)
        {
            _intoBuilder = new IntoBuilder<TLine, string>(CsvFileParser, columnName);
            _asBuilder = new AsBuilder<TLine>(CsvFileParser, columnName);
            _inThisWhayBuilder = new InThisWhayBuilder<TLine, string>(CsvFileParser, columnName);
        }
    }
}