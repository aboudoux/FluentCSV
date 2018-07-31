namespace FluentCsv.CsvParser.Splitters
{
    public interface IDataSplitter
    {
        string[] SplitColumns(string input, string columnDelimiter);

        string[] SplitLines(string input, string lineDelimiter);

        string GetFirstLine(string input, string lineDelimiter);

        void EnsureDelimitersAreValid(string lineDelimiter, string columnDelimiter);
    }
}