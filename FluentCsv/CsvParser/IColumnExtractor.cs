namespace FluentCsv.CsvParser
{
    public interface IColumnExtractor
    {
        void Extract(object result, string columnData);

        int ColumnIndex { get; }
        string ColumnName { get; }
    }
}