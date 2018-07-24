namespace FluentCsv.Exceptions
{
    public class DuplicateColumnNameException : FluentCsvException
    {
        public DuplicateColumnNameException(string columnName)
            : base($"the column name {columnName} is duplicated")
        {            
        }
    }
}