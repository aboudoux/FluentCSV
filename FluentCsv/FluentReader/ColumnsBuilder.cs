using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Results;

namespace FluentCsv.FluentReader
{
    public class ColumnsBuilder<TLine, TResultSet> 
        where TLine : new()
        where TResultSet : class, ICsvResult<TLine>
    {
        public ColumnsBuilder(CsvFileParser<TLine> parser, TResultSet resultSet)
        {
            Put = new ColumnFluentBuilder<TLine, TResultSet>(parser, resultSet);
        }

        public ColumnFluentBuilder<TLine, TResultSet> Put { get; }
    }
}