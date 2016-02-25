using System;
using JustAssembly.Core.DiffItems;
using JustAssembly.Core.DiffItems.Attributes;
using JustAssembly.Core.Extensions;
using Mono.Cecil;

namespace JustAssembly.Core.Comparers
{
    class CustomAttributeComparer : BaseDiffComparer<CustomAttribute>
    {
        protected override IDiffItem GetMissingDiffItem(CustomAttribute element)
        {
            return new CustomAttributeDiffItem(element, null);
        }

        protected override IDiffItem GenerateDiffItem(CustomAttribute oldElement, CustomAttribute newElement)
        {
            return null;
        }

        protected override IDiffItem GetNewDiffItem(CustomAttribute element)
        {
            return new CustomAttributeDiffItem(null, element);
        }

        protected override int CompareElements(CustomAttribute x, CustomAttribute y)
        {
            return x.Constructor.GetSignature().CompareTo(y.Constructor.GetSignature());
        }

        protected override bool IsAPIElement(CustomAttribute element)
        {
            return true;
        }
    }
}
