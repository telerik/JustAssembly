using System;
using System.Collections.Generic;
using System.Linq;
using JustAssembly.Core.DiffItems;
using JustAssembly.Core.DiffItems.Common;
using JustAssembly.Core.DiffItems.Types;
using JustAssembly.Core.Extensions;
using Mono.Cecil;


namespace JustAssembly.Core.Comparers
{
    class TypeComparer : BaseDiffComparer<TypeDefinition>
    {
        protected override IDiffItem GetMissingDiffItem(TypeDefinition element)
        {
            return new TypeDiffItem(element, null, null, null);
        }

        protected override IDiffItem GenerateDiffItem(TypeDefinition oldElement, TypeDefinition newElement)
        {
            IEnumerable<IDiffItem> attributeDiffs = new CustomAttributeComparer().GetMultipleDifferences(oldElement.CustomAttributes, newElement.CustomAttributes);

            IEnumerable<IDiffItem> declarationDiffs =
                EnumerableExtensions.ConcatAll<IDiffItem>(
                    attributeDiffs,
                    CheckVisibility(oldElement, newElement)
                );

            IEnumerable<IDiffItem> childrenDiffs =
                EnumerableExtensions.ConcatAll<IDiffItem>(
                    GetFieldDifferences(oldElement, newElement),
                    GetPropertyDifferences(oldElement, newElement),
                    GetMethodDifferences(oldElement, newElement),
                    GetEventDifferences(oldElement, newElement),
                    GetNestedTypeDiffs(oldElement, newElement)
                );

            if (declarationDiffs.IsEmpty() && childrenDiffs.IsEmpty())
            {
                return null;
            }
            return new TypeDiffItem(oldElement, newElement, declarationDiffs, childrenDiffs.Cast<IMetadataDiffItem>());
        }

        private IEnumerable<IDiffItem> CheckVisibility(TypeDefinition oldType, TypeDefinition newType)
        {
            int result = VisibilityComparer.CompareTypes(oldType, newType);
            if (result != 0)
            {
                yield return new VisibilityChangedDiffItem(result < 0);
            }
        }

        private IEnumerable<IDiffItem> GetMethodDifferences(TypeDefinition oldType, TypeDefinition newType)
        {
            return new MethodComparer().GetMultipleDifferences(oldType.Methods.Where(IsNotAccessor), newType.Methods.Where(IsNotAccessor));
        }

        private IEnumerable<IDiffItem> GetFieldDifferences(TypeDefinition oldType, TypeDefinition newType)
        {
            List<IDiffItem> result = new List<IDiffItem>(new FieldComparer().GetMultipleDifferences(oldType.Fields, newType.Fields));
            return result;
        }

        private IEnumerable<IDiffItem> GetPropertyDifferences(TypeDefinition oldType, TypeDefinition newType)
        {
            return new PropertyComparer().GetMultipleDifferences(oldType.Properties, newType.Properties);
        }

        private IEnumerable<IDiffItem> GetEventDifferences(TypeDefinition oldType, TypeDefinition newType)
        {
            return new EventComparer().GetMultipleDifferences(oldType.Events, newType.Events);
        }

        private IEnumerable<IDiffItem> GetNestedTypeDiffs(TypeDefinition oldType, TypeDefinition newType)
        {
            return new TypeComparer().GetMultipleDifferences(oldType.NestedTypes, newType.NestedTypes);
        }

        private static bool IsNotAccessor(MethodDefinition methodDef)
        {
            return !methodDef.IsGetter && !methodDef.IsSetter && !methodDef.IsAddOn && !methodDef.IsRemoveOn;
        }

        protected override IDiffItem GetNewDiffItem(TypeDefinition element)
        {
            return new TypeDiffItem(null, element, null, null);
        }

        protected override int CompareElements(TypeDefinition x, TypeDefinition y)
        {
            return x.FullName.CompareTo(y.FullName);
        }

        protected override bool IsAPIElement(TypeDefinition element)
        {
            return element.IsPublic || element.IsNestedPublic || element.IsNestedFamily || element.IsNestedFamilyOrAssembly;
        }
    }
}
