using System;
using System.Linq.Expressions;
using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Results;

namespace FluentCsv.FluentReader
{
	public class ChoiceBetweenAsAndInto<TLine, TResultSet> : ParserContainer<TLine, TResultSet> 
		where TLine : new()
		where TResultSet : class, ICsvResult<TLine>
	{
		private readonly IntoBuilder<TLine, string, TResultSet> _intoBuilder;

		private readonly AsBuilder<TLine, TResultSet> _asBuilder;

		private readonly InThisWayBuilder<TLine, string, TResultSet> _inThisWayBuilder;

		internal ChoiceBetweenAsAndInto(CsvFileParser<TLine> parser, int columnIndex, TResultSet resultSet, Func<string, Data> validation = null) : base(parser, resultSet)
		{
			_intoBuilder = new IntoBuilder<TLine, string, TResultSet>(CsvFileParser, columnIndex, resultSet, validator: validation);
			_asBuilder = new AsBuilder<TLine, TResultSet>(CsvFileParser, columnIndex, resultSet, validation);
			_inThisWayBuilder = new InThisWayBuilder<TLine, string, TResultSet>(CsvFileParser, columnIndex, resultSet, validation);
		}

		internal ChoiceBetweenAsAndInto(CsvFileParser<TLine> parser, string columnName, TResultSet resultSet, Func<string, Data> validation = null) : base(parser, resultSet)
		{
			_intoBuilder = new IntoBuilder<TLine, string, TResultSet>(CsvFileParser, columnName, resultSet,validator: validation);
			_asBuilder = new AsBuilder<TLine, TResultSet>(CsvFileParser, columnName, resultSet, validation);
			_inThisWayBuilder = new InThisWayBuilder<TLine, string, TResultSet>(CsvFileParser, columnName, resultSet, validation);
		}

		public AsMethods<TLine, T, TResultSet> As<T>()
		{
			return _asBuilder.As<T>();
		}

		public IntoConstraints<TLine, TResultSet> Into(Expression<Func<TLine, string>> intoMember)
		{
			return _intoBuilder.Into(intoMember);
		}

		public IntoBuilder<TLine, string, TResultSet> InThisWay(Func<string, string> inThisWay)
		{
			return _inThisWayBuilder.InThisWay(inThisWay);
		}
	}
}