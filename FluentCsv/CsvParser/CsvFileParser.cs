using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentCsv.Exceptions;

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

        public ParseCsvResult<TResult> Parse()
        {
            var currentLineNumber = 1;
            var resultSet = SplitLines(_source)
                .Skip(HeaderIfExists())
                .Select(line => _columns.GetResult(SplitColumns(line), currentLineNumber++))
                .ToList();

            return new ParseCsvResult<TResult>(
                resultSet.OfType<TResult>().ToArray(), 
                resultSet.OfType<CsvParseError>().ToArray());

            int HeaderIfExists()
            {
                if (_headerIndex == null)
                    return 0;
                currentLineNumber++;
                return 1;
            }
        }

        private string[] SplitLines(string source) 
            => source.Split(new[] {LineDelimiter}, StringSplitOptions.None);

        private string[] SplitColumns(string source) 
            => source.Split(new[] { ColumnDelimiter }, StringSplitOptions.None);

        private class HeaderIndex
        {
            private readonly Dictionary<string, int> _headerindex = new Dictionary<string, int>();
            private readonly HashSet<string> _duplicateColumnName = new HashSet<string>();

            public HeaderIndex(string[] headers)
            {
                var columnIndex = 0;

                headers.ForEach(MapHeaderToIndex);

                void MapHeaderToIndex(string header)
                {
                    var headerName = header.Trim();
                    if (_headerindex.ContainsKey(headerName))
                        _duplicateColumnName.Add(headerName);
                    else
                        _headerindex.Add(headerName, columnIndex);
                    columnIndex++;
                }
            }

            public int GetColumnIndex(string columnName)
            {
                var headerName = columnName.Trim();

                if (_duplicateColumnName.Contains(headerName))
                    throw new DuplicateColumnNameException(headerName);
                if(!_headerindex.ContainsKey(headerName))
                    throw new ColumnNameNotFoundException(headerName);
                return _headerindex[headerName];
            }
        }
    }

    public class ParseCsvResult<TResult>
    {
        public ParseCsvResult(TResult[] resultSet, CsvParseError[] errors)
        {
            ResultSet = resultSet;
            Errors = errors;
        }

        public TResult[] ResultSet { get; }
        public CsvParseError[] Errors { get; }
    }

    public class CsvParseError
    {
        public CsvParseError(int lineNumber, int columnNumber, string columnName, string errorMessage)
        {
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
            ColumnName = columnName;
            ErrorMessage = errorMessage;
        }

        public int LineNumber { get; }
        public int ColumnNumber { get; }
        public string ColumnName { get; }
        public string ErrorMessage { get; }
    }
}