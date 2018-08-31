using System.Globalization;
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
        private readonly FileParametersConstraints _choice;

        internal FluentFileParameters(CsvParameters csvParameters) : base(csvParameters)
        {
            _choice = new FileParametersConstraints(csvParameters, this);
        }

        public FileParametersConstraints ColumnsDelimiter(string delimiter)
        {
            CsvParameters.ColumnDelimiter = delimiter;
            return _choice;
        }

        public FileParametersConstraints EndOfLineDelimiter(string lineDelimiter)
        {
            CsvParameters.EndLineDelimiter = lineDelimiter;
            return _choice;
        }

        public FileParametersConstraints Header(As option = As.CaseInsensitive)
        {
            CsvParameters.FirstLineHasHeader = true;
            if (option == As.CaseSensitive)
                CsvParameters.HeaderCaseInsensitive = false;
            return _choice;
        }

        public FileParametersConstraints SimpleParsingMode()
        {
            CsvParameters.DataSplitter = new SimpleDataSplitter();
            return _choice;
        }

        public FileParametersConstraints CultureInfo(string culture)
        {
            CsvParameters.CultureInfo = new CultureInfo(culture);
            return _choice;
        }
    }
}