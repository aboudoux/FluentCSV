namespace FluentCsv.FluentReader
{
    public class ChoiceBetweenFileParametersrAndResultsetBuilder : CsvParametersContainer
    {
        public FluentFileParameters And { get; }
        public ResultSetBuilder That { get; }

        internal ChoiceBetweenFileParametersrAndResultsetBuilder(CsvParameters csvParameters, FluentFileParameters fileParameters) : base(csvParameters)
        {
            And = fileParameters;
            That = new ResultSetBuilder(csvParameters);
        }
    }
}