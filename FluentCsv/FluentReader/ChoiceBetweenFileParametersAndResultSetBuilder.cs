namespace FluentCsv.FluentReader
{
    public class ChoiceBetweenFileParametersAndResultSetBuilder : CsvParametersContainer
    {
        internal ChoiceBetweenFileParametersAndResultSetBuilder(CsvParameters csvParameters) : base(csvParameters)
        {
        }
        
        public FluentFileParameters With => new FluentFileParameters(CsvParameters);
        public ResultSetBuilder ThatReturns => new ResultSetBuilder(CsvParameters);
    }
}