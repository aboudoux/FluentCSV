using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using FluentCsv.Exceptions;
using FluentCsv.FluentReader;

namespace FluentCsv.CsvParser
{
    public class ColumnsResolver<TResult> where TResult : new()
    {
        private readonly CultureInfo _culture;

        public ColumnsResolver(CultureInfo culture)
        {
            _culture = culture ?? throw new ArgumentNullException(nameof(culture));
        }

        private readonly Dictionary<int, IColumnExtractor<TResult>> _columns = new Dictionary<int, IColumnExtractor<TResult>>();

        public void AddColumn<TMember>(int index, Expression<Func<TResult, TMember>> into, Func<string, TMember> setInThisWay = null, string columnName = null, Func<string, Data> dataValidator = null)
        {            
            VerifyColumnIndexIsUnique();

            var extractor = typeof(TResult).Name.StartsWith("ValueTuple")
                ? new TupleColumnExtractor<TResult, TMember>(index, _culture, columnName) 
                : new ColumnExtractor<TResult, TMember>(index, _culture, columnName);

            extractor.SetInto(into);

            if(setInThisWay != null)
                extractor.SetInThisWay(setInThisWay);

            if(dataValidator != null)
                extractor.SetValidator(dataValidator);
            
            _columns.Add(index, extractor);

            void VerifyColumnIndexIsUnique()
            {
                if (_columns.ContainsKey(index))
                    throw new ColumnWithSameIndexAlreadyDeclaredException(index);
            }
        }

        public object GetResult(string[] rawColumnsData, int lineNumber)
        {
            try
            {
                if(rawColumnsData.IsEmpty())
                    throw new CsvExtractException(0, lineNumber, "The line is empty");

                var input = new TResult() as object;
                var error = _columns.Values.Select(ExtractData).FirstOrDefault(a=>a != null);
                return (object)error ?? input;

                CsvParseError ExtractData(IColumnExtractor<TResult> extractor)
                {
                    try
                    {
                        var validation = extractor.Extract(input, rawColumnsData[extractor.ColumnIndex], out input);
                        if(validation is InvalidData invalid)
							return new CsvParseError(lineNumber, extractor.ColumnIndex, extractor.ColumnName, invalid.Reason);
                        return null;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new CsvExtractException(extractor.ColumnIndex, lineNumber,
                            $"The column at index {extractor.ColumnIndex} does not exists for line number {lineNumber}",
                            extractor.ColumnName);
                    }
                    catch (NullPropertyInstanceException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {                        
                        throw new CsvExtractException(extractor.ColumnIndex, lineNumber, e.Message, extractor.ColumnName);
                    }
                }
            }
            catch (CsvExtractException parseError)
            {                
                return new CsvParseError(parseError.LineNumber, parseError.ColumnIndex, parseError.ColumnName, parseError.Message);
            }
        }
    }

    public class CsvExtractException : Exception
    {
        public int ColumnIndex { get; }
        public string ColumnName { get; }
        public int LineNumber { get; }

        public CsvExtractException(int columnIndex, int lineNumber, string message, string columnName = null) : base(message)
        {
            ColumnIndex = columnIndex;
            LineNumber = lineNumber;
            ColumnName = columnName;
        }
    }
}