using FluentCsv.CsvParser.Splitters;

namespace FluentCsv.FluentReader
{
    public sealed class CsvParameters
    {
        public string Source { get; set; }
        public string ColumnDelimiter { get; set; } = ";";
        public string EndLineDelimiter { get; set; } = "\r\n";

        public bool FirstLineHasHeader = false;

        public IDataSplitter DataSplitter { get; set; } = new Rfc4180DataSplitter();
    }
}