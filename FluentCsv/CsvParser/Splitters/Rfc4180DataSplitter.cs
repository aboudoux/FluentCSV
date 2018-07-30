using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentCsv.Exceptions;

namespace FluentCsv.CsvParser.Splitters
{
    public class Rfc4180DataSplitter : IDataSplitter
    {
        private const string DoubleQuote = "\"\"";
        private const string Quote = "\"";
        
        public string[] SplitColumns(string input, string columnDelimiter)
        {
            var regex = string.Format("(?<=(^|{0})(?<quote>\"?))([^\"]|(\"\"))*?(?=\\<quote>(?={0}|$))", columnDelimiter);

            return Regex.Matches(input, regex)
                .Cast<Match>()
                .Select(a => ArrangeQuotes(a.Value))
                .ToArray();

            string ArrangeQuotes(string value)
            {
                return value == DoubleQuote ? string.Empty : value.Replace(DoubleQuote, Quote).Replace(DoubleQuote, Quote);
            }
        }

        public string[] SplitLines(string input, string lineDelimiter)
        {            
            if(string.IsNullOrWhiteSpace(input))
                return new[]{string.Empty};

            var splitIndex = GetAllDelimiterIndexNotBetweenQuotes(input, lineDelimiter);

            var previousIndex = 0;
            return splitIndex.Select(index =>
            {
                var substring = input.Substring(previousIndex, index - previousIndex);
                previousIndex = index + lineDelimiter.Length; ;
                return substring;
            }).ToArray();
        }

        private IEnumerable<int> GetAllDelimiterIndexNotBetweenQuotes(string input, string lineDelimiter)
        {
            var allQuotes = GetAllQuotesIndex(input);

            var result = new List<int>();

            var delimiterIndex = 0;
            while ((delimiterIndex = input.IndexOf(lineDelimiter, delimiterIndex + 1)) != -1)
            {
                if (allQuotes.All(a => a.NotIncluded(delimiterIndex)))
                    result.Add(delimiterIndex);
            }

            result.Add(input.Length);

            return result;
        }

        private static IEnumerable<BoundedQuotes> GetAllQuotesIndex(string input)
        {
            var sanitizedString = input.Replace(DoubleQuote, string.Empty);

            var result = new List<BoundedQuotes>();

            var bq = BoundedQuotes.Empty;
            while ((bq = ExtractBoundedQuotesIndexes(sanitizedString, bq.EndQuoteIndex+1)) != null)
                result.Add(bq);

            return result;
        }

        private static BoundedQuotes ExtractBoundedQuotesIndexes(string input, int startIndex)
        {            
            var startQuoteIndex = input.IndexOf(Quote, startIndex);
            if (startQuoteIndex < 0)
                return null;

            var stopQuoteIndex = input.IndexOf(Quote, startQuoteIndex + 1);
            if(stopQuoteIndex < 0)
                throw new MissingQuoteException();
            return new BoundedQuotes(startQuoteIndex, stopQuoteIndex);
        }        
    }

   
    internal class BoundedQuotes
    {
        public static BoundedQuotes Empty => new BoundedQuotes(0,0);

        public int StartQuoteIndex { get; }
        public int EndQuoteIndex { get; }

        public BoundedQuotes(int startQuoteIndex, int endQuoteIndex)
        {
            StartQuoteIndex = startQuoteIndex;
            EndQuoteIndex = endQuoteIndex;
        }

        public bool NotIncluded(int index)
            => !(index >= StartQuoteIndex && index <= EndQuoteIndex);
    }
}