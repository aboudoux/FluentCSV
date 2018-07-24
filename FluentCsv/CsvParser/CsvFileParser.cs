using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FluentCsv.CsvParser
{
    public class CsvFileParser<TResult> where TResult : new()
    {
        public string LineDelimiter { get; set; } = "\r\n";
        public string ColumnDelimiter { get; set; } = ";";

        private HeaderIndex _headerIndex;

        private readonly string _source;

        public CsvFileParser(string source)
        {
            _source = source;
        }

        public void DeclareFirstLineHasHeader()
            => _headerIndex = _headerIndex ?? new HeaderIndex(SplitColumns(SplitLines(_source).First()));

        private readonly ColumnsResolver<TResult> _columns = new ColumnsResolver<TResult>();

        public void AddColumn<TMember>(int index, Expression<Func<TResult, TMember>> into, Func<string, TMember> setInThisWay = null) 
            => _columns.AddColumn(index, into, setInThisWay);

        public void AddColumn<TMember>(string columnName, Expression<Func<TResult, TMember>> into, Func<string, TMember> setInThisWay = null)
        {
            DeclareFirstLineHasHeader();
            _columns.AddColumn(_headerIndex.GetColumnIndex(columnName), into, setInThisWay);
        }

        public TResult[] Parse()
        {
            return SplitLines(_source)
                  .Skip(HeaderIfExists())
                  .Select(line => _columns.GetResult(SplitColumns(line)))
                  .ToArray();

            int HeaderIfExists() => _headerIndex == null ? 0 : 1;
        }

        private string[] SplitLines(string source) 
            => source.Split(new[] {LineDelimiter}, StringSplitOptions.None);

        private string[] SplitColumns(string source) 
            => source.Split(new[] { ColumnDelimiter }, StringSplitOptions.None);

        private class HeaderIndex
        {
            private readonly Dictionary<string, int> _headerindex = new Dictionary<string, int>();

            public HeaderIndex(string[] headers)
            {
                var columnIndex = 0;
                _headerindex = headers.ToDictionary(a => a, a => columnIndex++);
            }

            public int GetColumnIndex(string header) => _headerindex[header];
        }
    }
}