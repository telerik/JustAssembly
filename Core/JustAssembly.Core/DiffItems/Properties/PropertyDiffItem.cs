using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace JustAssembly.Core.DiffItems.Properties
{
    class PropertyDiffItem : BaseMemberDiffItem<PropertyDefinition>
    {
        public PropertyDiffItem(PropertyDefinition oldProperty, PropertyDefinition newProperty, IEnumerable<IDiffItem> declarationDiffs, IEnumerable<IMetadataDiffItem> childrenDiffs)
            : base(oldProperty, newProperty, declarationDiffs, childrenDiffs)
        {
        }

        public override MetadataType MetadataType
        {
            get { return Core.MetadataType.Property; }
        }
    }
}
