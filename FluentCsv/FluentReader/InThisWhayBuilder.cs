using System;
using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Results;

namespace FluentCsv.FluentReader
{
    public class InThisWhayBuilder<TLine, TMember, TResultSet> : ParserContainer<TLine, TResultSet> 
        where TLine : new()
        where TResultSet : class, ICsvResult<TLine>
    {
        private readonly int _columnIndex;
        private readonly string _columnName;

        internal InThisWhayBuilder(CsvFileParser<TLine> csvParameters, int columnIndex, TResultSet resultSet) : base(csvParameters, resultSet)
        {
            _columnIndex = columnIndex;
        }
        internal InThisWhayBuilder(CsvFileParser<TLine> csvParameters, string columnName, TResultSet resultSet) : base(csvParameters, resultSet)
        {
            _columnName = columnName;
        }

        public IntoBuilder<TLine, TMember, TResultSet> InThisWay(Func<string, TMember> factory)
        {
            return _columnName.IsEmpty()
                ? new IntoBuilder<TLine, TMember, TResultSet>(CsvFileParser, _columnIndex, ResultSet, factory)
                : new IntoBuilder<TLine, TMember, TResultSet>(CsvFileParser, _columnName, ResultSet, factory);
        }
    }
}