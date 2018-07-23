using System;
using FluentCsv.CsvParser;

namespace FluentCsv.FluentReader
{
    public class InThisWhayBuilder<TLine, TMember> : ParserContainer<TLine> where TLine : new()
    {
        private readonly int _columnIndex;

        internal InThisWhayBuilder(CsvFileParser<TLine> csvParameters, int columnIndex) : base(csvParameters)
        {
            _columnIndex = columnIndex;
        }

        public IntoBuilder<TLine, TMember> InThisWay(Func<string, TMember> factory)
        {
            return new IntoBuilder<TLine, TMember>(CsvFileParser, _columnIndex, factory);
        }
    }
}