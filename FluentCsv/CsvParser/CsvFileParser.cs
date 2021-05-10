using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using FluentCsv.CsvParser.Results;
using FluentCsv.CsvParser.Splitters;
using FluentCsv.Exceptions;
using FluentCsv.FluentReader;

namespace FluentCsv.CsvParser
{
    public class CsvFileParser<TResult> where TResult : new()
    {
        private string _lineDelimiter = Environment.NewLine;
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

        public bool HeadersAsCaseInsensitive { get; set; } = false;

        public CsvFileParser(string source, IDataSplitter dataSplitter, CultureInfo cultureInfo)
        {
            _source = source;
            _dataSplitter = dataSplitter ?? throw new ArgumentNullException(nameof(dataSplitter));
            _columns = new ColumnsResolver<TResult>(cultureInfo);
        }

        public void DeclareFirstLineHasHeader()
        {
            _dataSplitter.EnsureDelimitersAreValid(LineDelimiter, ColumnDelimiter);
            _headerIndex ??= new HeaderIndex(SplitColumns(GetFirstLine(_source)), HeadersAsCaseInsensitive);
        }

        private readonly ColumnsResolver<TResult> _columns;
        
        public void AddColumn<TMember>(int index, Expression<Func<TResult, TMember>> into, Func<string, TMember> setInThisWay = null, Func<string, Data> dataValidator = null) 
            => _columns.AddColumn(index, into, setInThisWay, _headerIndex?.GetColumnName(index), dataValidator);

        public void AddColumn<TMember>(string columnName, Expression<Func<TResult, TMember>> into, Func<string, TMember> setInThisWay = null, Func<string, Data> dataValidator = null)
        {
            DeclareFirstLineHasHeader();
            _columns.AddColumn(_headerIndex.GetColumnIndex(columnName), into, setInThisWay, columnName, dataValidator);
        }

        public T Parse<T>(T result) where T : ICsvResult<TResult>
        {
            _dataSplitter.EnsureDelimitersAreValid(LineDelimiter, ColumnDelimiter);

			var currentLineNumber = 1;
			var resultSet = SplitLines(_source.TrimEnd(LineDelimiter.ToCharArray()))
				.Select(line=>(line,currentLineNumber++))
				.Skip(HeaderIfExists())
				.AsParallel()
				.Select(item => (_columns.GetResult(SplitColumns(item.line), item.Item2), item.Item2))
				.OrderBy(a=>a.Item2)
				.Select(a=>a.Item1)
				.ToList();

            result.Fill(
				resultSet.OfType<TResult>(), 
				resultSet.OfType<CsvParseError>().ToArray());

	        return result;

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
            private readonly bool _caseInsensitive;
            private readonly Dictionary<string, int> _headerToIndex = new Dictionary<string, int>();
            private readonly Dictionary<int, string> _indexToHeader = new Dictionary<int, string>();
            private readonly HashSet<string> _duplicateColumnName = new HashSet<string>();

            public HeaderIndex(string[] headers, bool caseInsensitive)
            {
                _caseInsensitive = caseInsensitive;
                var columnIndex = 0;

                headers.ForEach(MapHeaderToIndex);

                void MapHeaderToIndex(string header)
                {
                    var headerName = GetFinalHeaderName(header);

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
                var headerName = GetFinalHeaderName(columnName);

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

            private string GetFinalHeaderName(string originalHeaderName)
            {
                var headerName = originalHeaderName.Trim();
                if (_caseInsensitive)
                    headerName = headerName.ToLower();
                return headerName;
            }
        }
    }
}