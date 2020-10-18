using FluentCsv.FluentReader;

namespace FluentCsv.CsvParser
{
    public interface IColumnExtractor
    {
        Data Extract(object result, string columnData);

        int ColumnIndex { get; }
        string ColumnName { get; }
    }
}