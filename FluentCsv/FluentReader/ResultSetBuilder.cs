using FluentCsv.CsvParser;

namespace FluentCsv.FluentReader
{
    public class ResultSetBuilder : CsvParametersContainer
    {
        internal ResultSetBuilder(CsvParameters csvParameters) : base(csvParameters){}

        public ColumnsBuilder<TLine> ReturnsLinesOf<TLine>() where TLine : new()
        {
            var parser = new CsvFileParser<TLine>(CsvParameters.Source)
            {
                ColumnDelimiter = CsvParameters.ColumnDelimiter,
                LineDelimiter = CsvParameters.EndLineDelimiter
            };
            return new ColumnsBuilder<TLine>(parser);
        }
    }
}