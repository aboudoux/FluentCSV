using FluentCsv.CsvParser.Splitters;

namespace FluentCsv.FluentReader
{
    public enum As
    {
        CaseSensitive = 1,
        CaseInsensitive = 2,
    }

    public class FluentFileParameters : CsvParametersContainer
    {
        private readonly ChoiceBetweenFileParametersrAndResultsetBuilder _choice;

        internal FluentFileParameters(CsvParameters csvParameters) : base(csvParameters)
        {
            _choice = new ChoiceBetweenFileParametersrAndResultsetBuilder(csvParameters, this);
        }

        public ChoiceBetweenFileParametersrAndResultsetBuilder ColumnsDelimiter(string delimiter)
        {
            CsvParameters.ColumnDelimiter = delimiter;
            return _choice;
        }

        public ChoiceBetweenFileParametersrAndResultsetBuilder EndOfLineDelimiter(string lineDelimiter)
        {
            CsvParameters.EndLineDelimiter = lineDelimiter;
            return _choice;
        }

        public ChoiceBetweenFileParametersrAndResultsetBuilder Header(As option = As.CaseInsensitive)
        {
            CsvParameters.FirstLineHasHeader = true;
            if (option == As.CaseSensitive)
                CsvParameters.HeaderCaseInsensitive = false;
            return _choice;
        }

        public ChoiceBetweenFileParametersrAndResultsetBuilder SimpleParsingMode()
        {
            CsvParameters.DataSplitter = new SimpleDataSplitter();
            return _choice;
        }
    }
}