namespace FluentCsv.Exceptions
{
    public class ColumnWithSameIndexAlreadyDeclaredException : FluentCsvException
    {
        public ColumnWithSameIndexAlreadyDeclaredException(int index) 
            : base($"A column with index {index} is already declared")
        {
        }
    }
}
