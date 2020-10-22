using FluentCsv.FluentReader;

namespace FluentCsv.CsvParser
{
    public interface IColumnExtractor<TResult>
    {
        Data Extract(object source, string columnData, out object result);
        int ColumnIndex { get; }
        string ColumnName { get; }
    }
}