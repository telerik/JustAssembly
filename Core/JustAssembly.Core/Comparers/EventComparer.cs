using System;
using System.Collections.Generic;
using System.Linq;
using JustAssembly.Core.Comparers.Accessors;
using JustAssembly.Core.DiffItems;
using JustAssembly.Core.DiffItems.Events;
using JustAssembly.Core.Extensions;
using Mono.Cecil;

namespace JustAssembly.Core.Comparers
{
    class EventComparer : BaseDiffComparer<EventDefinition>
    {
        protected override IDiffItem GetMissingDiffItem(EventDefinition element)
        {
            return new EventDiffItem(element, null, null, null);
        }

        protected override IDiffItem GenerateDiffItem(EventDefinition oldElement, EventDefinition newElement)
        {
            IEnumerable<IDiffItem> declarationDiffs = new CustomAttributeComparer().GetMultipleDifferences(oldElement.CustomAttributes, newElement.CustomAttributes);
            IEnumerable<IMetadataDiffItem<MethodDefinition>> childrenDiffs = GenerateAccessorDifferences(oldElement, newElement);

            if (declarationDiffs.IsEmpty() && childrenDiffs.IsEmpty())
            {
                return null;
            }

            return new EventDiffItem(oldElement, newElement, declarationDiffs, childrenDiffs);
        }

        private IEnumerable<IMetadataDiffItem<MethodDefinition>> GenerateAccessorDifferences(EventDefinition oldEvent, EventDefinition newEvent)
        {
            List<IMetadataDiffItem<MethodDefinition>> result = new List<IMetadataDiffItem<MethodDefinition>>(2);

            IMetadataDiffItem<MethodDefinition> addAccessorDiffItem = new AddAccessorComparer(oldEvent, newEvent).GenerateAccessorDiffItem();
            if (addAccessorDiffItem != null)
            {
                result.Add(addAccessorDiffItem);
            }

            IMetadataDiffItem<MethodDefinition> removeAccessorDiffItem = new RemoveAccessorComparer(oldEvent, newEvent).GenerateAccessorDiffItem();
            if(removeAccessorDiffItem != null)
            {
                result.Add(removeAccessorDiffItem);
            }

            return result;
        }

        protected override IDiffItem GetNewDiffItem(EventDefinition element)
        {
            return new EventDiffItem(null, element, null, null);
        }

        protected override int CompareElements(EventDefinition x, EventDefinition y)
        {
            return x.Name.CompareTo(y.Name);
        }

        protected override bool IsAPIElement(EventDefinition element)
        {
            return element.AddMethod != null && element.AddMethod.IsAPIDefinition() ||
                element.RemoveMethod != null && element.RemoveMethod.IsAPIDefinition();
        }
    }
}
