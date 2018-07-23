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

        public TResult GetResult(string[] rawColumns)
        {
            var result = new TResult();
            foreach (var extractor in _columns)
                extractor.Value.Extract(result, rawColumns[extractor.Key]);
            return result;
        }
    }
}