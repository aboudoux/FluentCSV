using System;

namespace FluentCsv.CsvParser.Splitters
{
    public class SimpleDataSplitter : IDataSplitter
    {
        public string[] SplitColumns(string input, string columnDelimiter)        
            => input.RemoveBomIfExists().Split(new[] { columnDelimiter }, StringSplitOptions.None);

        public string[] SplitLines(string input, string lineDelimiter)
            => input.RemoveBomIfExists().Split(new[] { lineDelimiter }, StringSplitOptions.None);

        public string GetFirstLine(string input, string lineDelimiter)
        {
	        var inputWithoutBom = input.RemoveBomIfExists();
            var firstIndex = inputWithoutBom.IndexOf(lineDelimiter);
            return inputWithoutBom.Substring(0, firstIndex == -1 ? inputWithoutBom.Length : firstIndex );
        }

        public void EnsureDelimitersAreValid(string lineDelimiter, string columnDelimiter)
        { } // Do nothing : all delimiters are valid
    }
}