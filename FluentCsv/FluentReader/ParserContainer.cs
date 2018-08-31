using System;
using FluentCsv.CsvParser;
using FluentCsv.CsvParser.Results;

namespace FluentCsv.FluentReader
{
    public abstract class ParserContainer<T, TResultSet> 
        where T : new()
        where TResultSet : class, ICsvResult<T>
    {
        protected CsvFileParser<T> CsvFileParser { get; }
        protected TResultSet ResultSet { get; }

        protected ParserContainer(CsvFileParser<T> parser, TResultSet resultSet)
        {
            CsvFileParser = parser;
            ResultSet = resultSet ?? throw new ArgumentNullException(nameof(resultSet));
        }
    }
}