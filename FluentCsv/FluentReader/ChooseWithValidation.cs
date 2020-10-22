using System;
using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Results;

namespace FluentCsv.FluentReader
{
	public class ChooseWithValidation<TLine, TResultSet> : ChoiceBetweenAsAndInto<TLine, TResultSet>
		where TLine : new()
		where TResultSet : class, ICsvResult<TLine> 
	{
		private readonly CsvFileParser<TLine> _parser;
		private readonly string _columnName;
		private readonly int _columnIndex = -1;
		private readonly TResultSet _resultSet;

		internal ChooseWithValidation(CsvFileParser<TLine> parser, int columnIndex, TResultSet resultSet) : base(parser, columnIndex, resultSet)
		{
			_parser = parser;
			_columnIndex = columnIndex;
			_resultSet = resultSet;
		}

		internal ChooseWithValidation(CsvFileParser<TLine> parser, string columnName, TResultSet resultSet) : base(parser, columnName, resultSet)
		{
			_parser = parser;
			_columnName = columnName;
			_resultSet = resultSet;
		}

		public ChoiceBetweenAsAndInto<TLine, TResultSet> MakingSureThat(Func<string, Data> dataValidation) 
		{
			return _columnIndex == -1
				? new ChoiceBetweenAsAndInto<TLine, TResultSet>(_parser, _columnName, _resultSet, dataValidation)
				: new ChoiceBetweenAsAndInto<TLine, TResultSet>(_parser, _columnIndex, _resultSet, dataValidation);
		}
	}
}