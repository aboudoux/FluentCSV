using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Results;

namespace FluentCsv.FluentReader
{
    public class IntoConstraints<TLine, TResultSet> : ParserContainer<TLine, TResultSet> 
        where TLine : new()
        where TResultSet : class, ICsvResult<TLine>
    {
        internal IntoConstraints(CsvFileParser<TLine> csvParameters, TResultSet resultSet) : base(csvParameters, resultSet)
        {
            Put = new ColumnFluentBuilder<TLine, TResultSet>(CsvFileParser, resultSet);
        }

        public ColumnFluentBuilder<TLine, TResultSet> Put { get; }

        public TResultSet GetAll()
        {
            return CsvFileParser.Parse(ResultSet);
        }
    }
}