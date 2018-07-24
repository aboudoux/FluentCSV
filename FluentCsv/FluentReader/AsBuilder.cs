using FluentCsv.CsvParser;

namespace FluentCsv.FluentReader
{
    public class AsBuilder<TLine> : ParserContainer<TLine> where TLine : new()
    {
        private readonly int _columnIndex;
        private readonly string _columnName;

        internal AsBuilder(CsvFileParser<TLine> parser, int columnIndex) : base(parser)
        {
            _columnIndex = columnIndex;
        }
        internal AsBuilder(CsvFileParser<TLine> parser, string columnName) : base(parser)
        {
            _columnName = columnName;
        }

        public ChoiceBetweenInThisWayAndInto<TLine, T> As<T>()
        {
            return string.IsNullOrWhiteSpace(_columnName)
                ? new ChoiceBetweenInThisWayAndInto<TLine, T>(CsvFileParser, _columnIndex)
                : new ChoiceBetweenInThisWayAndInto<TLine, T>(CsvFileParser, _columnName);
        }
    }
}