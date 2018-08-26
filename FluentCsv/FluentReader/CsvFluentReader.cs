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
            _csvParameters.Source = File.ReadAllText(fileName, encoding ?? Encoding.Default).RemoveBomIfExists();
            return new ChoiceBetweenFileParametersAndResultSetBuilder(_csvParameters);
        }

	    public ChoiceBetweenFileParametersAndResultSetBuilder FromBytes(byte[] array, Encoding encoding = null)
	    {
		    var selectedEncoding = encoding ?? Encoding.Default;
		    _csvParameters.Source = selectedEncoding.GetString(array).RemoveBomIfExists();
		    return new ChoiceBetweenFileParametersAndResultSetBuilder(_csvParameters);
	    }

	    public ChoiceBetweenFileParametersAndResultSetBuilder FromStream(Stream stream, Encoding encoding = null) {

		    using (var reader = new StreamReader(stream, encoding ?? Encoding.Default))
		    {
			    reader.ReadToEnd();
			    _csvParameters.Source = reader.ReadToEnd().RemoveBomIfExists();
			    return new ChoiceBetweenFileParametersAndResultSetBuilder(_csvParameters);
			}
	    }

		public ChoiceBetweenFileParametersAndResultSetBuilder FromString(string @string)
        {
            _csvParameters.Source = @string.RemoveBomIfExists();
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
                        return reader.ReadToEnd().RemoveBomIfExists();
                    }
                }
            }
        }
    }
}