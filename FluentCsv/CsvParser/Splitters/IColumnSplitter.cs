namespace FluentCsv.CsvParser.Splitters
{
    public interface IColumnSplitter
    {
        string[] Split(string input, string columnDelimiter);
    }
}