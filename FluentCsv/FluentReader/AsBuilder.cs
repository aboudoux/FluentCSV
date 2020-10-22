using System;
using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Results;

namespace FluentCsv.FluentReader
{
    public class AsBuilder<TLine, TResultSet> : ParserContainer<TLine, TResultSet> 
        where TLine : new()
        where TResultSet : class, ICsvResult<TLine>
    {
        private readonly int _columnIndex;
        private readonly Func<string, Data> _dataValidation;
        private readonly string _columnName;

        internal AsBuilder(CsvFileParser<TLine> parser, int columnIndex, TResultSet resultSet, Func<string, Data> dataValidation = null) : base(parser, resultSet)
        {
	        _columnIndex = columnIndex;
	        _dataValidation = dataValidation;
        }
        internal AsBuilder(CsvFileParser<TLine> parser, string columnName, TResultSet resultSet , Func<string, Data> dataValidation = null) : base(parser, resultSet)
        {
	        _columnName = columnName;
	        _dataValidation = dataValidation;
        }

        public AsMethods<TLine, T, TResultSet> As<T>()
        {
            return _columnName.IsEmpty()
                ? new AsMethods<TLine, T, TResultSet>(CsvFileParser, _columnIndex, ResultSet, _dataValidation)
                : new AsMethods<TLine, T, TResultSet>(CsvFileParser, _columnName, ResultSet, _dataValidation);
        }
    }
}