using System;
using FluentCsv.CsvParser;

namespace FluentCsv.FluentReader
{
    public class InThisWhayBuilder<TLine, TMember> : ParserContainer<TLine> where TLine : new()
    {
        private readonly int _columnIndex;
        private readonly string _columnName;

        internal InThisWhayBuilder(CsvFileParser<TLine> csvParameters, int columnIndex) : base(csvParameters)
        {
            _columnIndex = columnIndex;
        }
        internal InThisWhayBuilder(CsvFileParser<TLine> csvParameters, string columnName) : base(csvParameters)
        {
            _columnName = columnName;
        }

        public IntoBuilder<TLine, TMember> InThisWay(Func<string, TMember> factory)
        {
            return string.IsNullOrWhiteSpace(_columnName)
                ? new IntoBuilder<TLine, TMember>(CsvFileParser, _columnIndex, factory)
                : new IntoBuilder<TLine, TMember>(CsvFileParser, _columnName, factory);
        }
    }
}