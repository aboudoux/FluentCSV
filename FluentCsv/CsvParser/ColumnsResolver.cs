using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentCsv.Exceptions;

namespace FluentCsv.CsvParser
{
    public class ColumnsResolver<TResult> where TResult : new()
    {
        private readonly Dictionary<int, IColumnExtractor> _columns = new Dictionary<int, IColumnExtractor>();

        public void AddColumn<TMember>(int index, Expression<Func<TResult, TMember>> into, Func<string, TMember> setInThisWay = null)
        {            
            VerifyColumnIndexIsUnique();

            var extractor = new ColumnExtractor<TResult, TMember>();
            extractor.SetInto(into);

            if(setInThisWay != null)
                extractor.SetInThisWay(setInThisWay);
            
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
                var result = new TResult();
                _columns.ForEach(ExtractData);
                return result;

                void ExtractData(KeyValuePair<int, IColumnExtractor> extractor)
                {
                    try
                    {
                        extractor.Value.Extract(result, rawColumnsData[extractor.Key]);
                    }
                    catch (Exception e)
                    {                        
                        throw new CsvExtractException(extractor.Key, lineNumber, e.Message);
                    }
                }
            }
            catch (CsvExtractException parseError)
            {                
                return new CsvParseError(parseError.LineNumber, parseError.ColumnIndex, "", parseError.Message);
            }
        }
    }

    public class CsvExtractException : Exception
    {
        public int ColumnIndex { get; }
        public int LineNumber { get; }

        public CsvExtractException(int columnIndex, int lineNumber, string message) : base(message)
        {
            ColumnIndex = columnIndex;
            LineNumber = lineNumber;
        }
    }
}