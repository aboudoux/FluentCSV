using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Results;

namespace FluentCsv.FluentReader
{
    public class ColumnFluentBuilder<TLine, TResultSet> : ParserContainer<TLine, TResultSet> 
        where TLine : new()
        where TResultSet : class, ICsvResult<TLine>
    {
        public ColumnFluentBuilder(CsvFileParser<TLine> parser, TResultSet resultSet) : base(parser, resultSet)
        {
        }

        public ChooseWithValidation<TLine, TResultSet> Column(int index)
        {
            return new ChooseWithValidation<TLine, TResultSet>(CsvFileParser, index, ResultSet);
        }

        public ChooseWithValidation<TLine, TResultSet> Column(string columnName)
        {
            return new ChooseWithValidation<TLine, TResultSet>(CsvFileParser, columnName, ResultSet);
        }
    }
}