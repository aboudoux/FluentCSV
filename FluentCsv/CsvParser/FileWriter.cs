using System.IO;
using System.Text;

namespace FluentCsv.CsvParser
{
	public class FileWriter : IFileWriter
	{
		public void Write(string filePath, string data, Encoding encoding)
		{
			File.WriteAllText(filePath, data, encoding ?? Encoding.Default);
		}
	}
}