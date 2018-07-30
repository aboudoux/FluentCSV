namespace FluentCsv.CsvParser.Splitters
{
    public interface IDataSplitter
    {
        string[] SplitColumns(string input, string columnDelimiter);

        string[] SplitLines(string input, string lineDelimiter);
    }
}