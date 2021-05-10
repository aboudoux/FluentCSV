using System;
using System.Collections.Generic;
using System.Linq;
using FluentCsv.Exceptions;

namespace FluentCsv.CsvParser.Splitters
{
    public class Rfc4180DataSplitter : IDataSplitter
    {
	    private const char Quote = '"';
        private const string DoubleQuote = "\"\"";

	    public string[] SplitColumns(string input, string columnDelimiter)
        {
            if (!input.Contains(Quote))
	            return input.Split(columnDelimiter);

            var result = new List<string>();
            var inputSpan = input.AsSpan();
            var doubleQuote = DoubleQuote.AsSpan();

            var startIndex = 0;
            while (startIndex <= inputSpan.Length)
            {
	            var currentSlice = inputSpan.Slice(startIndex);
	            int end = 0;
	            if (currentSlice.IsEmpty)
	            {
                    result.Add(string.Empty);
	            }
	            else if (currentSlice[0] != Quote)
	            {
		            end = currentSlice.IndexOf(columnDelimiter);
		            result.Add(end == -1
			            ? currentSlice.ToString()
			            : currentSlice.Slice(0, end).ToString());
		            if (end == -1)
			            break;
	            }
	            else
	            {
		            end = GetCloseQuoteIndex(currentSlice);
		            var column = currentSlice.Slice(1, end);
		            result.Add(column.Contains(doubleQuote, StringComparison.InvariantCulture)
			            ? column.ToString().Replace(DoubleQuote, "\"")
			            : column.ToString());
		            end += 2;
	            }

	            startIndex += end + columnDelimiter.Length;
            }

            return result.ToArray();

            int GetCloseQuoteIndex(ReadOnlySpan<char> span)
            {
	            var segment = span.Slice(1);
	            var result = segment.IndexOf((Quote + columnDelimiter).AsSpan());
	            return result == -1
		            ? segment.Length - 1
		            : result;
            }
        }

        public string[] SplitLines(string input, string lineDelimiter)
        {
            if (input.IsEmpty())
	            return Array.Empty<string>();

            var previousIndex = 0;

			return GetLinesIndex(input, lineDelimiter)
				  .Select(SplitedLine)
				  .ToArray();

			string SplitedLine(int delimiterIndex) {
				var substring = input.Substring(previousIndex, delimiterIndex - previousIndex);
				previousIndex = delimiterIndex + lineDelimiter.Length;
				return substring;
			}
		}

        public string GetFirstLine(string input, string lineDelimiter)
        {
            return GetLinesIndex(input, lineDelimiter, false)
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

        private static int[] GetLinesIndex(string input, ReadOnlySpan<char> lineDelimiterSpan, bool returnAll = true)
        {
	        var result = new List<int>();

	        var startIndex = 0;

	        var currentSlice = input.Replace(DoubleQuote, "  ").AsSpan();

	        while (input.Length >= startIndex )
	        {
		        var firstQuoteIndex = currentSlice.IndexOf(Quote);
		        var delimiterIndex = currentSlice.IndexOf(lineDelimiterSpan);
		        if (firstQuoteIndex != -1 && firstQuoteIndex < delimiterIndex)
		        {
			        var lastQuoteIndex = currentSlice.Slice(firstQuoteIndex+1).IndexOf(Quote);
			        if (lastQuoteIndex == -1) throw new MissingQuoteException();
			        startIndex += firstQuoteIndex + lastQuoteIndex + 2;
			        currentSlice = currentSlice.Slice(firstQuoteIndex + lastQuoteIndex + 2);
		        }
		        else
		        {
			        startIndex += delimiterIndex;
			        result.Add(delimiterIndex == -1 ? input.Length : startIndex);
			        if (!returnAll) break;
			        if (delimiterIndex == -1) break;
			        currentSlice = currentSlice.Slice(delimiterIndex + lineDelimiterSpan.Length);
			        startIndex += lineDelimiterSpan.Length;
		        }
	        }
	        return result.ToArray();
        }
    }
}