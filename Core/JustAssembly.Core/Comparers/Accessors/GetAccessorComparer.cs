using System;
using System.Collections.Generic;
using JustAssembly.Core.DiffItems;
using JustAssembly.Core.DiffItems.Properties;
using Mono.Cecil;

namespace JustAssembly.Core.Comparers.Accessors
{
    class GetAccessorComparer : BaseAccessorComparer<PropertyDefinition>
    {
        public GetAccessorComparer(PropertyDefinition oldProperty, PropertyDefinition newProperty)
            : base(oldProperty, newProperty)
        {
        }

        protected override MethodDefinition SelectAccessor(PropertyDefinition element)
        {
            return element.GetMethod;
        }

        protected override IMetadataDiffItem<MethodDefinition> CreateAccessorDiffItem(IEnumerable<IDiffItem> declarationDiffs)
        {
            return new GetAccessorDiffItem(this.oldElement, this.newElement, declarationDiffs);
        }
    }
}
