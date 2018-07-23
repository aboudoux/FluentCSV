namespace FluentCsv.FluentReader
{
    public class ChoiceBetweenFileParametersAndResultSetBuilder : CsvParametersContainer
    {
        internal ChoiceBetweenFileParametersAndResultSetBuilder(CsvParameters csvParameters) : base(csvParameters)
        {
        }
        
        public FluentFileParameters Where => new FluentFileParameters(CsvParameters);
        public ResultSetBuilder That => new ResultSetBuilder(CsvParameters);
    }
}