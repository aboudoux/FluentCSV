using System;
using System.Collections.Generic;
using System.Text;

namespace FluentCsv
{
    internal static class CollectionExtensions
    {
        internal static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable) 
                action(element);            
        }
    }
}
