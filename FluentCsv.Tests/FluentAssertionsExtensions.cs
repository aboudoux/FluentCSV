using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace FluentCsv.Tests
{
    public static class FluentAssertionsExtensions
    {
        public static void ShouldContainEquivalentTo<T>(this IEnumerable<T> subject, params T[] expected)
        {
            if(!expected.All(e => subject.Any(source => IsEquivalentTo(source, e))))
                throw new Exception("Expected subject to contain equivalent to provided object");
        }

        private static bool IsEquivalentTo<T>(this T source, T expected)
        {
            try
            {
                source.Should().BeEquivalentTo(expected);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}