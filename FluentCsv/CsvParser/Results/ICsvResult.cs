using System.Collections.Generic;

namespace FluentCsv.CsvParser.Results
{
    public interface ICsvResult<in TInput>
    {
        void Fill(IEnumerable<TInput> resultSetData, CsvParseError[] errors);
    }
}