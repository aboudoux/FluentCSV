using System.Text;

namespace FluentCsv.CsvParser
{
	public interface IFileWriter
	{
		void Write(string filePath, string data, Encoding encoding);
	}
}