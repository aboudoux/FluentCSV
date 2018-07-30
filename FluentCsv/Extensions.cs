using System;
using System.Collections.Generic;

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
    }
}
