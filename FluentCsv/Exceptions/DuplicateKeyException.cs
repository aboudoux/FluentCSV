namespace FluentCsv.Exceptions
{
    public class DuplicateKeyException : FluentCsvException
    {
        public DuplicateKeyException(object key)
            : base($"The key '{key}' already exists.")
        {
        }
    }
}