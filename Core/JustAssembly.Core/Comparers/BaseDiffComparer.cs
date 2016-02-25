using System;
using System.Collections.Generic;
using JustAssembly.Core.DiffItems;

namespace JustAssembly.Core.Comparers
{
    abstract class BaseDiffComparer<T> where T : class
    {
        public IEnumerable<IDiffItem> GetMultipleDifferences(IEnumerable<T> oldElements, IEnumerable<T> newElements)
        {
            List<T> oldElementsSorted = new List<T>(oldElements);
            oldElementsSorted.Sort(CompareElements);

            List<T> newElementsSorted = new List<T>(newElements);
            newElementsSorted.Sort(CompareElements);

            int oldIndex;
            int newIndex;

            List<IDiffItem> result = new List<IDiffItem>();

            for (oldIndex = 0, newIndex = 0; oldIndex < oldElementsSorted.Count && newIndex < newElementsSorted.Count; )
            {
                T oldElement = oldElementsSorted[oldIndex];
                T newElement = newElementsSorted[newIndex];

                int compareResult = CompareElements(oldElement, newElement);

                if (compareResult < 0)
                {
                    oldIndex++;
                    if (IsAPIElement(oldElement))
                    {
                        result.Add(GetMissingDiffItem(oldElement));
                    }
                }
                else if (compareResult > 0)
                {
                    newIndex++;
                    if (IsAPIElement(newElement))
                    {
                        IDiffItem newItem = GetNewDiffItem(newElement);
                        if (newItem != null)
                        {
                            result.Add(newItem);
                        }
                    }
                }
                else
                {
                    oldIndex++;
                    newIndex++;
                    if (IsAPIElement(oldElement) || IsAPIElement(newElement))
                    {
                        IDiffItem diffResult = this.GenerateDiffItem(oldElement, newElement);
                        if (diffResult != null)
                        {
                            result.Add(diffResult);
                        }
                    }
                }
            }

            for (; oldIndex < oldElementsSorted.Count; oldIndex++)
            {
                if (IsAPIElement(oldElementsSorted[oldIndex]))
                {
                    result.Add(GetMissingDiffItem(oldElementsSorted[oldIndex]));
                }
            }

            for (; newIndex < newElementsSorted.Count; newIndex++)
            {
                if (IsAPIElement(newElementsSorted[newIndex]))
                {
                    IDiffItem newItem = GetNewDiffItem(newElementsSorted[newIndex]);
                    if (newItem != null)
                    {
                        result.Add(newItem);
                    }
                }
            }

            return result;
        }

        protected abstract IDiffItem GetMissingDiffItem(T element);

        protected abstract IDiffItem GenerateDiffItem(T oldElement, T newElement);

        protected abstract IDiffItem GetNewDiffItem(T element);

        protected abstract bool IsAPIElement(T element);

        protected abstract int CompareElements(T x, T y);
    }
}
