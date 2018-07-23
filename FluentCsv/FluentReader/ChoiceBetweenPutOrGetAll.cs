using System.Collections.Generic;
using FluentCsv.CsvParser;

namespace FluentCsv.FluentReader
{
    public class ChoiceBetweenPutOrGetAll<TLine> : ParserContainer<TLine> where TLine : new()
    {
        public ColumnFluentBuilder<TLine> Put { get; }

        public IReadOnlyList<TLine> GetAll()
        {
            return CsvFileParser.Parse();
        }

        internal ChoiceBetweenPutOrGetAll(CsvFileParser<TLine> csvParameters) : base(csvParameters)
        {
            Put = new ColumnFluentBuilder<TLine>(CsvFileParser);
        }
    }
}