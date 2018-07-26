using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Splitters;

namespace FluentCsv.FluentReader
{
    public class ResultSetBuilder : CsvParametersContainer
    {        
        internal ResultSetBuilder(CsvParameters csvParameters) : base(csvParameters){}                

        public ColumnsBuilder<TLine> ReturnsLinesOf<TLine>() where TLine : new()
        {
            var parser = new CsvFileParser<TLine>(CsvParameters.Source, CsvParameters.ColumnSplitter)
            {
                ColumnDelimiter = CsvParameters.ColumnDelimiter,
                LineDelimiter = CsvParameters.EndLineDelimiter
            };

            if(CsvParameters.FirstLineHasHeader)
                parser.DeclareFirstLineHasHeader();

            return new ColumnsBuilder<TLine>(parser);
        }
    }
}