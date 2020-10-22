using System;
using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Results;

namespace FluentCsv.FluentReader
{
    public class InThisWayBuilder<TLine, TMember, TResultSet> : ParserContainer<TLine, TResultSet> 
        where TLine : new()
        where TResultSet : class, ICsvResult<TLine>
    {
        private readonly int _columnIndex;
        private readonly Func<string, Data> _dataValidation;
        private readonly string _columnName;

        internal InThisWayBuilder(CsvFileParser<TLine> csvParameters, int columnIndex, TResultSet resultSet, Func<string, Data> dataValidation = null) : base(csvParameters, resultSet)
        {
	        _columnIndex = columnIndex;
	        _dataValidation = dataValidation;
        }
        internal InThisWayBuilder(CsvFileParser<TLine> csvParameters, string columnName, TResultSet resultSet, Func<string, Data> dataValidation = null) : base(csvParameters, resultSet)
        {
	        _columnName = columnName;
	        _dataValidation = dataValidation;
        }

        public IntoBuilder<TLine, TMember, TResultSet> InThisWay(Func<string, TMember> factory)
        {
            return _columnName.IsEmpty()
                ? new IntoBuilder<TLine, TMember, TResultSet>(CsvFileParser, _columnIndex, ResultSet, factory, _dataValidation)
                : new IntoBuilder<TLine, TMember, TResultSet>(CsvFileParser, _columnName, ResultSet, factory, _dataValidation);
        }
    }
}