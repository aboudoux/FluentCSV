using System.Collections.Generic;
using System.Text;

namespace FluentCsv.CsvParser.Results
{
    public abstract class ParseCsvResult<TInput, TResult> : ICsvResult<TInput> 
        where TInput : new() 
    {
	    private readonly IFileWriter _fileWriter;

	    protected ParseCsvResult(IFileWriter fileWriter = null)
        {
	        _fileWriter = fileWriter ?? new FileWriter();
        }

		public TResult ResultSet { get; private set; }

		public CsvParseError[] Errors { get; private set; }

	    public void Fill(IEnumerable<TInput> resultSetData, CsvParseError[] errors)
	    {
		    ResultSet = GetFinalResult(resultSetData);
		    Errors = errors;
	    }

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

        protected abstract TResult GetFinalResult(IEnumerable<TInput> input);
    }
}