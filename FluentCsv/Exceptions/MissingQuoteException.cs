namespace FluentCsv.Exceptions
{
    public class MissingQuoteException : FluentCsvException
    {
        public MissingQuoteException() 
            : base("the csv file has data in quotes, and one of them can not be found. For disable this error, disable the structured csv parsinf with Rfc4180IsNotUsedForParsing method")
        {            
        }
    }
}