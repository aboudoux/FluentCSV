using System.Reflection;

namespace FluentCsv.Exceptions
{
    public class NullPropertyInstanceException : FluentCsvException
    {
        public NullPropertyInstanceException(PropertyInfo pinfo)
            : base($"A property of type class or struct of your resultset is null. Please, set all your subclass in your resultset with the new keyword. PropertyName : {pinfo.Name} - PropertyType : {pinfo.PropertyType}")
        {
        }
    }
}