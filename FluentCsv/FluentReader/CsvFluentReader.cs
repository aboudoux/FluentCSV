using System.IO;

namespace FluentCsv.FluentReader
{
    public class CsvFluentReader
    {
        private readonly CsvParameters _csvParameters = new CsvParameters();

        public ChoiceBetweenFileParametersAndResultSetBuilder FromFile(string fileName)
        {
            _csvParameters.Source = File.ReadAllText(fileName);
            return new ChoiceBetweenFileParametersAndResultSetBuilder(_csvParameters);
        }

        public ChoiceBetweenFileParametersAndResultSetBuilder FromString(string @string)
        {
            _csvParameters.Source = @string;
            return new ChoiceBetweenFileParametersAndResultSetBuilder(_csvParameters);
        }
    }
}