namespace FluentCsv.Exceptions
{
    public class BadDelimiterException : FluentCsvException
    {
        public BadDelimiterException(string forbiddenDelimiter) 
            : base($"your delimiter contains the forbidden characters '{forbiddenDelimiter}' that cannot be used for parsing the file. If you want to use it, use the 'With.SimpleParsingMode' configuration method to switch in simple parsing mode")
        {
            
        }
    }
}