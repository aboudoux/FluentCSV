using System;

namespace FluentCsv.CsvParser.Splitters
{
    public class SimpleColumnSplitter : IColumnSplitter
    {
        public string[] Split(string input, string columnDelimiter)        
            => input.Split(new[] { columnDelimiter }, StringSplitOptions.None);
    }
}