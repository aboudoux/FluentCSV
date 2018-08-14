using System.IO;
using System.Reflection;
using System.Text;
using FluentCsv.Exceptions;

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

        public ChoiceBetweenFileParametersAndResultSetBuilder FromAssemblyResource(string resourceName, Assembly assembly = null, Encoding encoding = null)
        {
            var currentAssembly = assembly ?? Assembly.GetCallingAssembly();
            _csvParameters.Source = ReadAllText(currentAssembly, resourceName);
            return new ChoiceBetweenFileParametersAndResultSetBuilder(_csvParameters);

            string ReadAllText(Assembly asm, string resource)
            {
                using (var stream = asm.GetManifestResourceStream(resource))
                {
                    if (stream == null)
                        throw new AssemblyResourceNotFoundException(asm, resource);

                    using (var reader = new StreamReader(stream, encoding ?? Encoding.Default))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}