namespace FluentCsv.FluentReader
{
    public class ChoiceBetweenFileParametersrAndResultsetBuilder : CsvParametersContainer
    {
        public FluentFileParameters And { get; }
        public ResultSetBuilder AndReturn { get; }

        internal ChoiceBetweenFileParametersrAndResultsetBuilder(CsvParameters csvParameters, FluentFileParameters fileParameters) : base(csvParameters)
        {
            And = fileParameters;
            AndReturn = new ResultSetBuilder(csvParameters);
        }
    }
}