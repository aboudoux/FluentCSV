using System.Linq;
using System.Text.RegularExpressions;

namespace FluentCsv.CsvParser.Splitters
{
    public class Rfc4180ColumnSplitter : IColumnSplitter
    {
        public string[] Split(string input, string columnDelimiter)
        {
            var regex = string.Format("(?<=(^|{0})(?<quote>\"?))([^\"]|(\"\"))*?(?=\\<quote>(?={0}|$))", columnDelimiter);

            return Regex.Matches(input, regex)
                .Cast<Match>()
                .Select(a => ArrangeQuotes(a.Value))
                .ToArray();

            string ArrangeQuotes(string value)
            {
                const string doubleQuote = "\"\"";
                return value == doubleQuote ? string.Empty : value.Replace(doubleQuote, "\"").Replace(doubleQuote, "\"");
            }
        }
    }
}