using System;
using System.Collections.Generic;
using System.Linq;
using JustAssembly.Core.DiffItems;
using JustAssembly.Core.DiffItems.Properties;
using Mono.Cecil;

namespace JustAssembly.Core.Comparers.Accessors
{
    class SetAccessorComparer : BaseAccessorComparer<PropertyDefinition>
    {
        public SetAccessorComparer(PropertyDefinition oldProperty, PropertyDefinition newProperty)
            : base(oldProperty, newProperty)
        {
        }

        protected override MethodDefinition SelectAccessor(PropertyDefinition element)
        {
            return element.SetMethod;
        }

        protected override IMetadataDiffItem<MethodDefinition> CreateAccessorDiffItem(IEnumerable<IDiffItem> declarationDiffs)
        {
            return new SetAccessorDiffItem(this.oldElement, this.newElement, declarationDiffs);
        }
    }
}
