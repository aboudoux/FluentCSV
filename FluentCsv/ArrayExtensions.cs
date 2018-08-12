using System;

namespace FluentCsv
{
    public static class ArrayExtensions
    {
        public static void ForEach<T>(this T[] collection, Action<T> action)
        {
            foreach (var element in collection)
                action(element);
        }
    }
}