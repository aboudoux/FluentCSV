using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentCsv
{
    internal static class Extensions
    {
        internal static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable) 
                action(element);            
        }

        internal static bool IsEmpty(this string source)
            => string.IsNullOrWhiteSpace(source);

        internal static bool IsEmptyWithWhiteSpaceAllowed(this string source)
            => string.IsNullOrEmpty(source);

        internal static IEnumerable<T> DequeueWhile<T>(this Queue<T> queue, Func<T, bool> predicate)
        {
            while (queue.Count != 0 && predicate(queue.Peek()))
                yield return queue.Dequeue();
        }

        internal static bool IsEmpty(this string[] source)
            => source.All(a => a.IsEmpty());

        internal static IEnumerable<T> WithoutLastElement<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
            var elementsCount = enumerable.Count();
            var currentCount = 1;
            foreach (var element in enumerable)
            {
                if (currentCount++ < elementsCount)
                    yield return element;
            }
        }

	    internal static string RemoveBomIfExists(this string source)
	    {
		    const string utf8Bom = "\uFEFF";

			if (source.IsEmpty())
			    return source;
		    return source.StartsWith(utf8Bom) ? source.Replace(utf8Bom,string.Empty) : source;
	    }
    }
}
