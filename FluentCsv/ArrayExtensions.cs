using System;
using System.Collections.Generic;

namespace FluentCsv
{
    public static class ArrayExtensions
    {
        public static void ForEach<T>(this T[] collection, Action<T> action)
        {
            foreach (var element in collection)
                action(element);
        }

        public static string[] Split(this ReadOnlySpan<char> input, ReadOnlySpan<char> delimiter)
        {
	        List<string> result = new List<string>();

	        if (input.SequenceEqual(delimiter))
		        return Array.Empty<string>();

	        if (delimiter.Length > input.Length || !input.Contains(delimiter, StringComparison.InvariantCulture))
		        return new[] {input.ToString()};

	        int startIndex = 0;
	        var delimiterIndex = 0;
	        do
	        {
		        var currentSlice = input.Slice(startIndex);
		        delimiterIndex = currentSlice.IndexOf(delimiter);
		        result.Add(delimiterIndex == -1 
			        ? currentSlice.Slice(0).ToString()
			        : currentSlice.Slice(0, delimiterIndex).ToString());
		        startIndex += delimiterIndex + delimiter.Length;

	        } while (delimiterIndex != -1);

	        return result.ToArray();
        }
    }
}