using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Results;

namespace FluentCsv.FluentReader
{
    public class AsBuilder<TLine, TResultSet> : ParserContainer<TLine, TResultSet> 
        where TLine : new()
        where TResultSet : class, ICsvResult<TLine>
    {
        private readonly int _columnIndex;
        private readonly string _columnName;

        internal AsBuilder(CsvFileParser<TLine> parser, int columnIndex, TResultSet resultSet) : base(parser, resultSet)
        {
            _columnIndex = columnIndex;
        }
        internal AsBuilder(CsvFileParser<TLine> parser, string columnName, TResultSet resultSet) : base(parser, resultSet)
        {
            _columnName = columnName;
        }

        public AsMethods<TLine, T, TResultSet> As<T>()
        {
            return _columnName.IsEmpty()
                ? new AsMethods<TLine, T, TResultSet>(CsvFileParser, _columnIndex, ResultSet)
                : new AsMethods<TLine, T, TResultSet>(CsvFileParser, _columnName, ResultSet);
        }
    }
}