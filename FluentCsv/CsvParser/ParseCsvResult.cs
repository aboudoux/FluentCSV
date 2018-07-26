namespace FluentCsv.CsvParser
{
    public class ParseCsvResult<TResult>
    {
        public ParseCsvResult(TResult[] resultSet, CsvParseError[] errors)
        {
            ResultSet = resultSet;
            Errors = errors;
        }

        public TResult[] ResultSet { get; }
        public CsvParseError[] Errors { get; }
    }
}