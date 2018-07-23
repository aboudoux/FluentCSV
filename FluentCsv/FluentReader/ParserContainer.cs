using FluentCsv.CsvParser;

namespace FluentCsv.FluentReader
{
    public abstract class ParserContainer<T> where T : new()
    {
        protected CsvFileParser<T> CsvFileParser { get; }

        protected ParserContainer(CsvFileParser<T> parser)
        {
            CsvFileParser = parser;
        }
    }
}