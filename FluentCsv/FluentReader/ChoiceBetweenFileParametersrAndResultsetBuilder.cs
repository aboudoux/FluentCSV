namespace FluentCsv.FluentReader
{
    public class ChoiceBetweenFileParametersrAndResultsetBuilder : CsvParametersContainer
    {
        public FluentFileParameters And { get; }
        public ResultSetBuilder ThatReturns { get; }

        internal ChoiceBetweenFileParametersrAndResultsetBuilder(CsvParameters csvParameters, FluentFileParameters fileParameters) : base(csvParameters)
        {
            And = fileParameters;
            ThatReturns = new ResultSetBuilder(csvParameters);
        }
    }
}