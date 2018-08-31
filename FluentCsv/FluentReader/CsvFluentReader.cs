using System.IO;
using System.Reflection;
using System.Text;
using FluentCsv.Exceptions;

namespace FluentCsv.FluentReader
{
    public class CsvFluentReader
    {
        private readonly CsvParameters _csvParameters = new CsvParameters();
		private readonly Encoding _encoding;

	    public CsvFluentReader(Encoding encoding)
	    {
		    _encoding = encoding;
	    }

	    public CsvFluentReader() : this(Encoding.Default)
	    {
	    }

        public FromConstraints FromFile(string fileName)
        {
            _csvParameters.Source = File.ReadAllText(fileName, _encoding).RemoveBomIfExists();
            return new FromConstraints(_csvParameters);
        }

	    public FromConstraints FromBytes(byte[] array)
	    {
		    _csvParameters.Source = _encoding.GetString(array).RemoveBomIfExists();
		    return new FromConstraints(_csvParameters);
	    }

	    public FromConstraints FromStream(Stream stream) {

		    using (var reader = new StreamReader(stream, _encoding))
		    {
			    _csvParameters.Source = reader.ReadToEnd().RemoveBomIfExists();
			    return new FromConstraints(_csvParameters);
			}
	    }

		public FromConstraints FromString(string @string)
        {
            _csvParameters.Source = @string.RemoveBomIfExists();
            return new FromConstraints(_csvParameters);
        }

        public FromConstraints FromAssemblyResource(string resourceName, Assembly assembly = null)
        {
            var currentAssembly = assembly ?? Assembly.GetCallingAssembly();
            _csvParameters.Source = ReadAllText(currentAssembly, resourceName);
            return new FromConstraints(_csvParameters);

            string ReadAllText(Assembly asm, string resource)
            {
                using (var stream = asm.GetManifestResourceStream(resource))
                {
                    if (stream == null)
                        throw new AssemblyResourceNotFoundException(asm, resource);

                    using (var reader = new StreamReader(stream, _encoding))
                    {
                        return reader.ReadToEnd().RemoveBomIfExists();
                    }
                }
            }
        }
    }

	public class CsvFluentReaderWithEncoding : CsvFluentReader
	{
		public CsvFluentReader EncodedIn(Encoding encoding)
		 => new CsvFluentReader(encoding);
	}
}