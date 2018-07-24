namespace FluentCsv.Exceptions
{
    public class ColumnNameNotFoundException : FluentCsvException
    {
        public ColumnNameNotFoundException(string columnName)
            : base($"The column name {columnName} does not exists in the csv file header")
        {            
        }
    }
}