namespace Fanex.NAP.SPU.CTS.Shares
{
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NAP.SPU.CTS.CC.Features.CustomerClassification.NormalAccountInsert.Entities;

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3240:The simplest possible condition syntax should be used", Justification = "<Pending>")]
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

        public static IEnumerable<T> MergeWithOtherEnumerable<T, V>(IEnumerable<T> source, IEnumerable<V> other, Func<T, V, T> funcMapping)
            where T : BaseCustTaggedModel, new()
            where V : BaseCustTaggedEntity, new()
        {
            var sourceMapping = source ?? new List<T>();
            var otherMapping = other ?? new List<V>();

            return sourceMapping.GroupJoin(otherMapping,
                               custClass => custClass.CustId,
                               taggedCust => taggedCust.CustID,
                               (custClass, taggedCusts) => new { custClass, taggedCusts })
                               .SelectMany(
                                    group => group.taggedCusts.DefaultIfEmpty(),
                                    (group, temp) => funcMapping(group.custClass, temp));
        }
    }
}
