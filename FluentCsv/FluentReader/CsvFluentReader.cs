using System.IO;
using System.Text;

namespace FluentCsv.FluentReader
{
    public class CsvFluentReader
    {
        private readonly CsvParameters _csvParameters = new CsvParameters();

        public ChoiceBetweenFileParametersAndResultSetBuilder FromFile(string fileName, Encoding encoding = null)
        {
            _csvParameters.Source = File.ReadAllText(fileName, encoding ?? Encoding.Default);
            return new ChoiceBetweenFileParametersAndResultSetBuilder(_csvParameters);
        }

        public ChoiceBetweenFileParametersAndResultSetBuilder FromString(string @string)
        {
            _csvParameters.Source = @string;
            return new ChoiceBetweenFileParametersAndResultSetBuilder(_csvParameters);
        }
    }
}