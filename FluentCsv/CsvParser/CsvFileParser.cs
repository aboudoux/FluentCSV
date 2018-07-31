using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentCsv.CsvParser.Splitters;
using FluentCsv.Exceptions;

namespace FluentCsv.CsvParser
{
    public class CsvFileParser<TResult> where TResult : new()
    {
        private string _lineDelimiter = "\r\n";
        private string _columnDelimiter = ";";

        private HeaderIndex _headerIndex;

        private readonly string _source;
        private readonly IDataSplitter _dataSplitter;

        public string LineDelimiter
        {
            get => _lineDelimiter;
            set
            {
                if(value.IsEmptyWithWhiteSpaceAllowed())
                    throw new EmptyLineDelimiterException();
                _lineDelimiter = value;
            }
        }

        public string ColumnDelimiter
        {
            get => _columnDelimiter;
            set
            {
                if(value.IsEmptyWithWhiteSpaceAllowed())
                    throw new EmptyColumnDelimiterException();
                _columnDelimiter = value;
            }
        }


        public CsvFileParser(string source, IDataSplitter dataSplitter)
        {
            _source = source;
            _dataSplitter = dataSplitter ?? throw new ArgumentNullException(nameof(dataSplitter));
        }

        public void DeclareFirstLineHasHeader()
        {
            _dataSplitter.EnsureDelimitersAreValid(LineDelimiter, ColumnDelimiter);
            _headerIndex = _headerIndex ?? new HeaderIndex(SplitColumns(GetFirstLine(_source)));
        }

        private readonly ColumnsResolver<TResult> _columns = new ColumnsResolver<TResult>();
        
        public void AddColumn<TMember>(int index, Expression<Func<TResult, TMember>> into, Func<string, TMember> setInThisWay = null) 
            => _columns.AddColumn(index, into, setInThisWay, _headerIndex?.GetColumnName(index));

        public void AddColumn<TMember>(string columnName, Expression<Func<TResult, TMember>> into, Func<string, TMember> setInThisWay = null)
        {
            DeclareFirstLineHasHeader();
            _columns.AddColumn(_headerIndex.GetColumnIndex(columnName), into, setInThisWay, columnName);
        }

        public ParseCsvResult<TResult> Parse()
        {
            _dataSplitter.EnsureDelimitersAreValid(LineDelimiter, ColumnDelimiter);

            var currentLineNumber = 1;
            var resultSet = SplitLines(_source.TrimEnd(LineDelimiter.ToCharArray()))
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

        public string[] SplitColumns(string line)
            => _dataSplitter.SplitColumns(line, ColumnDelimiter);

        private string[] SplitLines(string source)
            => _dataSplitter.SplitLines(source, LineDelimiter);

        private string GetFirstLine(string source)
            => _dataSplitter.GetFirstLine(source, LineDelimiter);

        private class HeaderIndex
        {
            private readonly Dictionary<string, int> _headerToIndex = new Dictionary<string, int>();
            private readonly Dictionary<int, string> _indexToHeader = new Dictionary<int, string>();
            private readonly HashSet<string> _duplicateColumnName = new HashSet<string>();

            public HeaderIndex(string[] headers)
            {
                var columnIndex = 0;

                headers.ForEach(MapHeaderToIndex);

                void MapHeaderToIndex(string header)
                {
                    var headerName = header.Trim();
                    if (_headerToIndex.ContainsKey(headerName))
                        _duplicateColumnName.Add(headerName);
                    else
                    {
                        _headerToIndex.Add(headerName, columnIndex);
                        _indexToHeader.Add(columnIndex, headerName);
                    }
                    columnIndex++;
                }
            }

            public int GetColumnIndex(string columnName)
            {
                var headerName = columnName.Trim();

                if (_duplicateColumnName.Contains(headerName))
                    throw new DuplicateColumnNameException(headerName);
                if(!_headerToIndex.ContainsKey(headerName))
                    throw new ColumnNameNotFoundException(headerName);
                return _headerToIndex[headerName];
            }

            public string GetColumnName(int index)
            {
                return _indexToHeader.ContainsKey(index) ? _indexToHeader[index] : null;
            }
        }
    }
}