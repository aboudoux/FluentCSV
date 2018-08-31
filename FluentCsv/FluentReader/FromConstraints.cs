namespace FluentCsv.FluentReader
{
    public class FromConstraints : CsvParametersContainer
    {
        internal FromConstraints(CsvParameters csvParameters) : base(csvParameters)
        {
        }
        
        public FluentFileParameters With => new FluentFileParameters(CsvParameters);
        public ResultSetBuilder ThatReturns => new ResultSetBuilder(CsvParameters);
    }
}