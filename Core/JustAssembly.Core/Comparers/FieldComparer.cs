using System;
using System.Collections.Generic;
using System.Linq;
using JustAssembly.Core.DiffItems;
using JustAssembly.Core.DiffItems.Common;
using JustAssembly.Core.DiffItems.Fields;
using JustAssembly.Core.Extensions;
using Mono.Cecil;

namespace JustAssembly.Core.Comparers
{
    class FieldComparer : BaseDiffComparer<FieldDefinition>
    {
        protected override IDiffItem GetMissingDiffItem(FieldDefinition element)
        {
            return new FieldDiffItem(element, null, null);
        }

        protected override IDiffItem GenerateDiffItem(FieldDefinition oldElement, FieldDefinition newElement)
        {
            IEnumerable<IDiffItem> attributeDiffs = new CustomAttributeComparer().GetMultipleDifferences(oldElement.CustomAttributes, newElement.CustomAttributes);
            IEnumerable<IDiffItem> fieldTypeDiffs = GetFieldTypeDiff(oldElement, newElement);

            IEnumerable<IDiffItem> declarationDiffs =
                EnumerableExtensions.ConcatAll(
                    attributeDiffs,
                    CheckVisibility(oldElement, newElement),
                    CheckStaticFlag(oldElement, newElement),
                    fieldTypeDiffs
                );

            if(declarationDiffs.IsEmpty())
            {
                return null;
            }
            return new FieldDiffItem(oldElement, newElement, declarationDiffs);
        }

        private IEnumerable<IDiffItem> CheckVisibility(FieldDefinition oldField, FieldDefinition newField)
        {
            int result = VisibilityComparer.CompareVisibilityDefinitions(oldField, newField);
            if (result != 0)
            {
                yield return new VisibilityChangedDiffItem(result < 0);
            }
        }

        private IEnumerable<IDiffItem> CheckStaticFlag(FieldDefinition oldField, FieldDefinition newField)
        {
            if (oldField.IsStatic != newField.IsStatic)
            {
                yield return new StaticFlagChangedDiffItem(newField.IsStatic);
            }
        }

        private IEnumerable<IDiffItem> GetFieldTypeDiff(FieldDefinition oldField, FieldDefinition newField)
        {
            if (oldField.FieldType.FullName != newField.FieldType.FullName)
            {
                yield return new MemberTypeDiffItem(oldField, newField);
            }
        }

        protected override IDiffItem GetNewDiffItem(FieldDefinition element)
        {
            return new FieldDiffItem(null, element, null);
        }

        protected override int CompareElements(FieldDefinition x, FieldDefinition y)
        {
            return x.Name.CompareTo(y.Name);
        }

        protected override bool IsAPIElement(FieldDefinition element)
        {
            return element.IsAPIDefinition();
        }
    }
}
