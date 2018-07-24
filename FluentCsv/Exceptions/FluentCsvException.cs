using System;
using System.Runtime.Serialization;

namespace FluentCsv.Exceptions
{
    public class FluentCsvException : Exception
    {
        public FluentCsvException()
        {
        }

        public FluentCsvException(string message) : base(message)
        {
        }

        public FluentCsvException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FluentCsvException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}