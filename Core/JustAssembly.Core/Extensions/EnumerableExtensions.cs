using System;
using System.Collections;
using System.Collections.Generic;

namespace JustAssembly.Core.Extensions
{
    static class EnumerableExtensions
    {
        public static bool IsEmpty(this IEnumerable self)
        {
            if (self == null)
            {
                throw new NullReferenceException();
            }

            ICollection collection = self as ICollection;
            return collection != null ? collection.Count == 0 : !self.GetEnumerator().MoveNext();
        }

        public static IEnumerable<T> ConcatAll<T>(params IEnumerable<T>[] enumerables)
        {
            foreach (IEnumerable<T> enumerable in enumerables)
            {
                foreach (T item in enumerable)
                {
                    yield return item;
                }
            }
        }
    }
}
