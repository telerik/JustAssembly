using System;
using System.Collections.Generic;
using System.Linq;
using JustAssembly.Interfaces;
using JustAssembly.MergeUtilities;

namespace JustAssembly.Extensions
{
    static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> self, IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return;
            }
            foreach (T item in enumerable)
            {
                self.Add(item);
            }
        }

        public static IEnumerable<IOldToNewTupleMap<T>> Merge<T>(this IEnumerable<T> oldTypes, IEnumerable<T> newTypes, Comparison<T> comparer)
            where T : class
        {
            IEnumerator<T> leftTypeDefEnumerator = oldTypes.GetEnumerator();

            IEnumerator<T> rigthTypeDefEnumerator = newTypes.GetEnumerator();

            leftTypeDefEnumerator.MoveNext();

            rigthTypeDefEnumerator.MoveNext();

            return Merge(leftTypeDefEnumerator, rigthTypeDefEnumerator, comparer);
        }

        private static IEnumerable<IOldToNewTupleMap<T>> Merge<T>(IEnumerator<T> oldTypesEnumerator, IEnumerator<T> newTypesEnumerator, Comparison<T> comparer)
             where T : class
        {
            while (true)
            {
                T leftType = oldTypesEnumerator.Current;

                T rightType = newTypesEnumerator.Current;

                if (leftType != null && rightType != null)
                {
                    int compareIndex = comparer(leftType, rightType);

                    if (compareIndex == 0)
                    {
                        yield return new OldToNewTupleMap<T>(leftType, rightType);

                        oldTypesEnumerator.MoveNext();

                        newTypesEnumerator.MoveNext();
                    }
                    else if (compareIndex < 0)
                    {
                        yield return new OldToNewTupleMap<T>(leftType, default(T));

                        oldTypesEnumerator.MoveNext();
                    }
                    else
                    {
                        yield return new OldToNewTupleMap<T>(default(T), rightType);

                        newTypesEnumerator.MoveNext();
                    }
                }
                else if (leftType != null)
                {
                    yield return new OldToNewTupleMap<T>(leftType, default(T));

                    oldTypesEnumerator.MoveNext();
                }
                else if (rightType != null)
                {
                    yield return new OldToNewTupleMap<T>(default(T), rightType);

                    newTypesEnumerator.MoveNext();
                }
                else
                {
                    yield break;
                }
            }
        }
    }
}
