using System.Collections.Generic;
using System.Linq;

namespace FluentCsv.CsvParser.Results
{
    public class ArrayCsvResult<TResult> : ParseCsvResult<TResult, TResult[]> 
        where TResult : new() 
    {
        public ArrayCsvResult(IFileWriter fileWriter = null) : base(fileWriter){}

        protected override TResult[] GetFinalResult(IEnumerable<TResult> input)
            => input.ToArray();
    }
}