using System;
using System.Collections.Generic;
using System.Linq;
using FluentCsv.Exceptions;

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

        internal static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<TValue> input, Func<TValue, TKey> keySelector)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            var result = new Dictionary<TKey, TValue>();
            input.ForEach(AddElementToDictionary);
            return result;

            void AddElementToDictionary(TValue element)
            {
                var key = keySelector(element);
                if (result.ContainsKey(key))
                    throw new DuplicateKeyException(key);
                result.Add(key, element);
            }
        }
        
	    internal static string RemoveBomIfExists(this string source) => source.TrimStart('\uFEFF', '\uFFFE');
    }
}
