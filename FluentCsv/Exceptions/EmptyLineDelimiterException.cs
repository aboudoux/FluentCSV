namespace FluentCsv.Exceptions
{
    public class EmptyLineDelimiterException : FluentCsvException
    {
        public EmptyLineDelimiterException()
            : base("the line delimiter cannot be empty")
        {
        }
    }
}