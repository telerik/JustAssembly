using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace JustAssembly.Core.DiffItems.Properties
{
    class SetAccessorDiffItem : BaseDiffItem<MethodDefinition>
    {
        public SetAccessorDiffItem(PropertyDefinition oldProperty, PropertyDefinition newProperty, IEnumerable<IDiffItem> declarationDiffs)
            : base(oldProperty.SetMethod, newProperty.SetMethod, declarationDiffs, null)
        {
        }

        public override MetadataType MetadataType
        {
            get { return Core.MetadataType.Method; }
        }

        protected override string GetElementShortName(MethodDefinition element)
        {
            return "set";
        }
    }
}
