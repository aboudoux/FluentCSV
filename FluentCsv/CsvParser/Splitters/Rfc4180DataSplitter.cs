using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentCsv.Exceptions;
using FluentCsv;

namespace FluentCsv.CsvParser.Splitters
{
    public class Rfc4180DataSplitter : IDataSplitter
    {
        private const string DoubleQuote = "\"\"";
        private const string Quote = "\"";
        
        public string[] SplitColumns(string input, string columnDelimiter)
        {
            EnsureDelimiterIsValid(columnDelimiter);

            var regex = string.Format("(?<=(^|{0})(?<quote>\"?))([^\"]|(\"\"))*?(?=\\<quote>(?={0}|$))", columnDelimiter);

            return Regex.Matches(input, regex)
                .Cast<Match>()
                .Select(a => ArrangeQuotes(a.Value))
                .ToArray();

            string ArrangeQuotes(string value)
                => value == DoubleQuote 
                    ? string.Empty 
                    : value.Replace(DoubleQuote, Quote).Replace(DoubleQuote, Quote);
        }

        public string[] SplitLines(string input, string lineDelimiter)
        {
            EnsureDelimiterIsValid(lineDelimiter);

            if (input.IsEmpty())
                return new[]{string.Empty};

            var previousIndex = 0;

            return GetAllNewLineDelimiterIndexThatArentBetweenQuotes(input, lineDelimiter)
                  .Select(SplittedLine)
                  .ToArray();

            string SplittedLine(int delimiterIndex)
            {
                var substring = input.Substring(previousIndex, delimiterIndex - previousIndex);
                previousIndex = delimiterIndex + lineDelimiter.Length;
                return substring;
            }
        }

        public string GetFirstLine(string input, string lineDelimiter)
        {
            EnsureDelimiterIsValid(lineDelimiter);

            return GetAllNewLineDelimiterIndexThatArentBetweenQuotes(input, lineDelimiter)
                .Select(firstIndex => input.Substring(0, firstIndex))
                .FirstOrDefault();
        }

        public void EnsureDelimitersAreValid(string lineDelimiter, string columnDelimiter)
        {
            EnsureDelimiterIsValid(lineDelimiter);
            EnsureDelimiterIsValid(columnDelimiter);
        }

        private static void EnsureDelimiterIsValid(string delimiter)
        {
            if(delimiter.Contains("\""))
                throw new BadDelimiterException("\"");
        }

        private static IReadOnlyList<int> GetAllNewLineDelimiterIndexThatArentBetweenQuotes(string input, string lineDelimiter)
        {
            const int notFound = -1;

            var allQuotes = GetAllQuotesIndex(input);

            var result = new List<int>();

            var delimiterIndex = 0;
            var quotesToTest = new List<BoundedQuotes>();

            while ((delimiterIndex = GetNewLineDelimiterIndex()) != notFound)
            {
                FeedOnlyBoundedQuotesLowerByDelimiterIndex();
                AddDelimiterIfNotBetweenQuotes();
                ClearQuotesToTestButKeepLastElement();
            }

            result.Add(input.Length);
            return result;

            int GetNewLineDelimiterIndex()
            {
                return input.IsEmpty()
                    ? notFound
                    : input.IndexOf(lineDelimiter, delimiterIndex + 1, StringComparison.Ordinal);
            }

            void FeedOnlyBoundedQuotesLowerByDelimiterIndex()
                => quotesToTest.AddRange(
                    allQuotes.DequeueWhile(a => a.StartQuoteIndex <= delimiterIndex));

            void AddDelimiterIfNotBetweenQuotes()
            {
                if (quotesToTest.All(a => a.NotIncluded(delimiterIndex)))
                    result.Add(delimiterIndex);
            }

            void ClearQuotesToTestButKeepLastElement()
            {
                if (quotesToTest.Any())
                    quotesToTest.RemoveRange(0, quotesToTest.Count - 1);
            }
        }

        private static Queue<BoundedQuotes> GetAllQuotesIndex(string input)
        {
            var sanitizedString = input.Replace(DoubleQuote, "  ");

            var result = new Queue<BoundedQuotes>();

            var bq = BoundedQuotes.Empty;
            while ((bq = ExtractBoundedQuotesIndexes(sanitizedString, bq.EndQuoteIndex+1)) != BoundedQuotes.NotFound)
                result.Enqueue(bq);

            return result;
        }

        private static BoundedQuotes ExtractBoundedQuotesIndexes(string input, int startIndex)
        {            
            var startQuoteIndex = input.IndexOf(Quote, startIndex, StringComparison.Ordinal);
            if (startQuoteIndex < 0)
                return BoundedQuotes.NotFound;

            var stopQuoteIndex = input.IndexOf(Quote, startQuoteIndex+1, StringComparison.Ordinal);
            if(stopQuoteIndex < 0)
                throw new MissingQuoteException();

            return new BoundedQuotes(startQuoteIndex, stopQuoteIndex);
        }

        private class BoundedQuotes
        {
            public static BoundedQuotes Empty => new BoundedQuotes(-1, -1);
            public static BoundedQuotes NotFound => null;

            internal int StartQuoteIndex { get; }
            internal int EndQuoteIndex { get; }

            public BoundedQuotes(int startQuoteIndex, int endQuoteIndex)
            {
                StartQuoteIndex = startQuoteIndex;
                EndQuoteIndex = endQuoteIndex;
            }

            public bool NotIncluded(int index)
                => !(index >= StartQuoteIndex && index <= EndQuoteIndex);
        }
    }
}