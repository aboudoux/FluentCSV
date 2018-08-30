using System.Text;

namespace FluentCsv.CsvParser
{
    public class ParseCsvResult<TResult>
    {
	    private readonly IFileWriter _fileWriter;

	    public ParseCsvResult(TResult[] resultSet, CsvParseError[] errors, IFileWriter fileWriter = null)
        {
	        _fileWriter = fileWriter ?? new FileWriter();
	        ResultSet = resultSet;
            Errors = errors;
        }

        public TResult[] ResultSet { get; }
        public CsvParseError[] Errors { get; }

	    public void SaveErrorsInFile(string csvFilePath, Encoding encoding = null)
	    {
		    const string header = "Line;ColumnZeroBaseIndex;ColumnName;Message\r\n";

			var fileData = new StringBuilder(header);
			Errors.ForEach(e=>
				fileData.AppendLine(
					$"{e.LineNumber};{e.ColumnZeroBasedIndex};{Enquote(e.ColumnName)};{Enquote(e.ErrorMessage)}"));

			_fileWriter.Write(csvFilePath, fileData.ToString(), encoding);

		    string Enquote(string source) => $"\"{source.Replace("\"","\"\"")}\"";
	    }
	}
}