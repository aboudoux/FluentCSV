using System;

namespace FluentCsv.Exceptions
{
    public class ColumnWithSameIndexAlreadyDeclaredException : Exception
    {
        public ColumnWithSameIndexAlreadyDeclaredException(int index) 
            : base($"A column with index {index} is already declared")
        {
        }
    }
}
