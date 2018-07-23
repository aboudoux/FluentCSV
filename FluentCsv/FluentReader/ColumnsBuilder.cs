using FluentCsv.CsvParser;

namespace FluentCsv.FluentReader
{
    public class ColumnsBuilder<TLine> where TLine : new()
    {
        public ColumnsBuilder(CsvFileParser<TLine> parser)
        {
            Put = new ColumnFluentBuilder<TLine>(parser);
        }

        public ColumnFluentBuilder<TLine> Put { get; }
    }
}