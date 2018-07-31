using System;

namespace FluentCsv.CsvParser.Splitters
{
    public class SimpleDataSplitter : IDataSplitter
    {
        public string[] SplitColumns(string input, string columnDelimiter)        
            => input.Split(new[] { columnDelimiter }, StringSplitOptions.None);

        public string[] SplitLines(string input, string lineDelimiter)
            => input.Split(new[] { lineDelimiter }, StringSplitOptions.None);

        public string GetFirstLine(string input, string lineDelimiter)
        {
            var firstIndex = input.IndexOf(lineDelimiter);
            return input.Substring(0, firstIndex == -1 ? input.Length : firstIndex );
        }

        public void EnsureDelimitersAreValid(string lineDelimiter, string columnDelimiter)
        { } // Do nothing : all delimiters are valid
    }
}