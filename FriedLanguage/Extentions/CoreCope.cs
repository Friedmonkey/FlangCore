using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriedLanguage
{
    public static class CoreCope
    {
        //public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
        //{
        //    TSource? first = source.TryGetFirst(out bool found);
        //    return found ? first! : defaultValue;
        //}
        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
        {
            foreach (var item in source)
            {
                return item;
            }
            return defaultValue;
        }
        //public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
        //{
        //    TSource? last = source.TryGetLast(out bool found);
        //    return found ? last! : defaultValue;
        //}
        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
        {
            TSource lastItem = defaultValue;
            bool found = false;
            foreach (var item in source)
            {
                lastItem = item;
                found = true;
            }
            if (!found)
            {
                return defaultValue;
            }
            return lastItem;
        }


        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair, out TKey key, out TValue value)
        {
            key = keyValuePair.Key;
            value = keyValuePair.Value;
        }
    }
}
