namespace FluentCsv.Exceptions
{
    public class MissingQuoteException : FluentCsvException
    {
        public MissingQuoteException() 
            : base("the csv file has data in quotes, and one of them cannot be found. For disable this error, use the 'Where.Rfc4180IsNotUsedForParsing' configuration method")
        {            
        }
    }
}