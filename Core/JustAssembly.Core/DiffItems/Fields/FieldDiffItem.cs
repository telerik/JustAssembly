using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace JustAssembly.Core.DiffItems.Fields
{
    class FieldDiffItem : BaseMemberDiffItem<FieldDefinition>
    {
        public FieldDiffItem(FieldDefinition oldField, FieldDefinition newField, IEnumerable<IDiffItem> declarationDiffs) 
            : base(oldField, newField, declarationDiffs, null)
        {
        }

        public override MetadataType MetadataType
        {
            get { return Core.MetadataType.Field; }
        }
    }
}
