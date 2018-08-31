using System;
using System.Collections.Generic;

namespace FluentCsv.CsvParser.Results
{
    public class DictionaryCsvResult<TResult> : ParseCsvResult<TResult, Dictionary<object, TResult>>
        where TResult : new()
    {
        private readonly Func<TResult, object> _keySelector;

        public DictionaryCsvResult(Func<TResult, object> keySelector, IFileWriter fileWriter = null) 
            : base(fileWriter)
        {
            _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        }

        protected override Dictionary<object, TResult> GetFinalResult(IEnumerable<TResult> input)
            => input.ToDictionary(_keySelector);
    }
}