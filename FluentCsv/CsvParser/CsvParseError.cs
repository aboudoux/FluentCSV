namespace FluentCsv.CsvParser
{
    public class CsvParseError
    {
        public CsvParseError(int lineNumber, int columnZeroBasedIndex, string columnName, string errorMessage)
        {
            LineNumber = lineNumber;
            ColumnZeroBasedIndex = columnZeroBasedIndex;
            ColumnName = columnName;
            ErrorMessage = errorMessage;
        }

        public int LineNumber { get; }
        public int ColumnZeroBasedIndex { get; }
        public string ColumnName { get; }
        public string ErrorMessage { get; }
    }
}