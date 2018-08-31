using System;
using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Results;

namespace FluentCsv.FluentReader
{
    public class ResultSetBuilder : CsvParametersContainer
    {        
        internal ResultSetBuilder(CsvParameters csvParameters) : base(csvParameters){}                

        public ColumnsBuilder<TLine, ArrayCsvResult<TLine>> ArrayOf<TLine>() 
            where TLine : new() 
            => new ColumnsBuilder<TLine, ArrayCsvResult<TLine>>(GetParser<TLine>(), new ArrayCsvResult<TLine>());

        public ColumnsBuilder<TLine, DictionaryCsvResult<TLine>> DictionaryOf<TLine>(Func<TLine,object> keySelector) 
            where TLine : new() 
            => new ColumnsBuilder<TLine, DictionaryCsvResult<TLine>>(GetParser<TLine>(), new DictionaryCsvResult<TLine>(keySelector));

        private CsvFileParser<TLine> GetParser<TLine>()
            where TLine : new()
        {
            var parser = new CsvFileParser<TLine>(CsvParameters.Source,
                CsvParameters.DataSplitter, CsvParameters.CultureInfo)
            {
                ColumnDelimiter = CsvParameters.ColumnDelimiter,
                LineDelimiter = CsvParameters.EndLineDelimiter,
                HeadersAsCaseInsensitive = CsvParameters.HeaderCaseInsensitive,
            };

            if (CsvParameters.FirstLineHasHeader)
                parser.DeclareFirstLineHasHeader();

            return parser;
        }
    }
}