namespace FluentCsv.FluentReader
{
    public class FluentFileParameters : CsvParametersContainer
    {
        private readonly ChoiceBetweenFileParametersrAndResultsetBuilder _choice;
        

        internal FluentFileParameters(CsvParameters csvParameters) : base(csvParameters)
        {
            _choice = new ChoiceBetweenFileParametersrAndResultsetBuilder(csvParameters, this);
        }

        public ChoiceBetweenFileParametersrAndResultsetBuilder ColumnsAreDelimitedBy(string delimiter)
        {
            CsvParameters.ColumnDelimiter = delimiter;
            return _choice;
        }

        public ChoiceBetweenFileParametersrAndResultsetBuilder LinesEndWith(string lineDelimiter)
        {
            CsvParameters.EndLineDelimiter = lineDelimiter;
            return _choice;
        }

        public ChoiceBetweenFileParametersrAndResultsetBuilder FirstLineIsHeader()
        {
            CsvParameters.FirstLineHasHeader = true;
            return _choice;
        }
    }
}