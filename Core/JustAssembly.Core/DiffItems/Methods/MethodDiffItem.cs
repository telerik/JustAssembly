using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace JustAssembly.Core.DiffItems.Methods
{
    class MethodDiffItem : BaseMemberDiffItem<MethodDefinition>
    {
        public MethodDiffItem(MethodDefinition oldMethod, MethodDefinition newMethod, IEnumerable<IDiffItem> declarationDiffs)
            : base(oldMethod, newMethod, declarationDiffs, null)
        {
        }

        public override MetadataType MetadataType
        {
            get { return Core.MetadataType.Method; }
        }
    }
}
