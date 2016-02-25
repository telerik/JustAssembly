using System;
using System.Collections.Generic;
using System.Linq;
using JustAssembly.Core.Comparers.Accessors;
using JustAssembly.Core.DiffItems;
using JustAssembly.Core.DiffItems.Common;
using JustAssembly.Core.DiffItems.Properties;
using JustAssembly.Core.Extensions;
using Mono.Cecil;

namespace JustAssembly.Core.Comparers
{
    class PropertyComparer : BaseDiffComparer<PropertyDefinition>
    {
        protected override IDiffItem GetMissingDiffItem(PropertyDefinition element)
        {
            return new PropertyDiffItem(element, null, null, null);
        }

        protected override IDiffItem GenerateDiffItem(PropertyDefinition oldElement, PropertyDefinition newElement)
        {
            IEnumerable<IDiffItem> declarationDiffs =
                EnumerableExtensions.ConcatAll(
                    new CustomAttributeComparer().GetMultipleDifferences(oldElement.CustomAttributes, newElement.CustomAttributes),
                    GetReturnTypeDifference(oldElement, newElement)
                    );
            IEnumerable<IMetadataDiffItem<MethodDefinition>> childrenDiffs = GenerateAccessorDifferences(oldElement, newElement);

            if (declarationDiffs.IsEmpty() && childrenDiffs.IsEmpty())
            {
                return null;
            }

            return new PropertyDiffItem(oldElement, newElement, declarationDiffs, childrenDiffs);
        }

        private IEnumerable<IDiffItem> GetReturnTypeDifference(PropertyDefinition oldProperty, PropertyDefinition newProperty)
        {
            if (oldProperty.PropertyType.FullName != newProperty.PropertyType.FullName)
            {
                yield return new MemberTypeDiffItem(oldProperty, newProperty);
            }
        }

        private IEnumerable<IMetadataDiffItem<MethodDefinition>> GenerateAccessorDifferences(PropertyDefinition oldProperty, PropertyDefinition newProperty)
        {
            List<IMetadataDiffItem<MethodDefinition>> result = new List<IMetadataDiffItem<MethodDefinition>>(2);

            IMetadataDiffItem<MethodDefinition> getAccessorDiffItem = new GetAccessorComparer(oldProperty, newProperty).GenerateAccessorDiffItem();
            if (getAccessorDiffItem != null)
            {
                result.Add(getAccessorDiffItem);
            }

            IMetadataDiffItem<MethodDefinition> setAccessorDiffItem = new SetAccessorComparer(oldProperty, newProperty).GenerateAccessorDiffItem();
            if (setAccessorDiffItem != null)
            {
                result.Add(setAccessorDiffItem);
            }

            return result;
        }

        protected override IDiffItem GetNewDiffItem(PropertyDefinition element)
        {
            return new PropertyDiffItem(null, element, null, null);
        }

        protected override int CompareElements(PropertyDefinition x, PropertyDefinition y)
        {
            return x.Name.CompareTo(y.Name);
        }

        protected override bool IsAPIElement(PropertyDefinition element)
        {
            return element.GetMethod != null && element.GetMethod.IsAPIDefinition() ||
                element.SetMethod != null && element.SetMethod.IsAPIDefinition();
        }
    }
}
