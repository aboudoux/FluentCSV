namespace FluentCsv.Exceptions
{
    public class EmptyColumnDelimiterException : FluentCsvException
    {
        public EmptyColumnDelimiterException()
            : base("the column delimiter cannot be empty")
        {
        }
    }
}