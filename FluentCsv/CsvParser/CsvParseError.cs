namespace FluentCsv.CsvParser
{
    public class CsvParseError
    {
        public CsvParseError(int lineNumber, int columnNumber, string columnName, string errorMessage)
        {
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
            ColumnName = columnName;
            ErrorMessage = errorMessage;
        }

        public int LineNumber { get; }
        public int ColumnNumber { get; }
        public string ColumnName { get; }
        public string ErrorMessage { get; }
    }
}