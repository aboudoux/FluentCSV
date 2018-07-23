using System;
using System.Linq;
using System.Linq.Expressions;

namespace FluentCsv.CsvParser
{
    public class CsvFileParser<TResult> where TResult : new()
    {
        public string LineDelimiter { get; set; } = "\r\n";
        public string ColumnDelimiter { get; set; } = ";";

        private readonly string _source;

        public CsvFileParser(string source)
        {
            _source = source;
        }

        private readonly ColumnsResolver<TResult> _columns = new ColumnsResolver<TResult>();

        public void AddColumn<TMember>(int index, Expression<Func<TResult, TMember>> into, Func<string, TMember> setInThisWay = null) 
            => _columns.AddColumn(index, into, setInThisWay);

        public TResult[] Parse()
        {
            return SplitLines(_source)
                  .Select(line => _columns.GetResult(SplitColumns(line)))
                  .ToArray();            
        }

        private string[] SplitLines(string source) 
            => source.Split(new[] {LineDelimiter}, StringSplitOptions.None);

        private string[] SplitColumns(string source) 
            => source.Split(new[] { ColumnDelimiter }, StringSplitOptions.None);
    }
}