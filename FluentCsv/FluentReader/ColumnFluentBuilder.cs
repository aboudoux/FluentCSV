using FluentCsv.CsvParser;

namespace FluentCsv.FluentReader
{
    public class ColumnFluentBuilder<TLine> : ParserContainer<TLine> where TLine : new()
    {
        public ColumnFluentBuilder(CsvFileParser<TLine> parser) : base(parser)
        {
        }

        public ChoiceBetweenAsAndInto<TLine> Column(int index)
        {
            return new ChoiceBetweenAsAndInto<TLine>(CsvFileParser, index);
        }

        public ChoiceBetweenAsAndInto<TLine> Column(string columnName)
        {
            return new ChoiceBetweenAsAndInto<TLine>(CsvFileParser, columnName);
        }
    }
}