using FluentCsv.CsvParser;

namespace FluentCsv.FluentReader
{
    public class AsBuilder<TLine> : ParserContainer<TLine> where TLine : new()
    {
        private readonly int _columnIndex;

        internal AsBuilder(CsvFileParser<TLine> parser, int columnIndex) : base(parser)
        {
            _columnIndex = columnIndex;
        }

        public ChoiceBetweenInThisWayAndInto<TLine, T> As<T>()
        {
            return new ChoiceBetweenInThisWayAndInto<TLine, T>(CsvFileParser, _columnIndex);
        }
    }
}