namespace Fanex.NAP.SPU.CTS.Shares
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public static class IEnumerableExtention
    {
        public static string Join<T, TR>(this IEnumerable<T> data, Func<T, TR> selector, string seperator = ",")
        {
            return string.Join(seperator, data.Select(selector));
        }

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> data, Func<TSource, object> predicate)
        {
            return from item in data.GroupBy(predicate) select item.First();
        }

        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> data)
        {
            return data != null && data.Any();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> data)
        {
            return !IsNotNullOrEmpty(data);
        }

        public static IEnumerable<TSource> WhereIf<TSource>(
            this IEnumerable<TSource> source,
            bool condition,
            Func<TSource, bool> predicate)
        {
            if (condition)
            {
                return source.Where(predicate);
            }
            else
            {
                return source;
            }
        }
    }
}
