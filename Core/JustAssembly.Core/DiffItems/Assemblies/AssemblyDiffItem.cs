using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace JustAssembly.Core.DiffItems.Assemblies
{
    class AssemblyDiffItem : BaseDiffItem<AssemblyDefinition>
    {
        public AssemblyDiffItem(AssemblyDefinition oldAssembly, AssemblyDefinition newAssembly,
            IEnumerable<IDiffItem> declarationDiffs, IEnumerable<IMetadataDiffItem<ModuleDefinition>> childrenDiffs)
            : base(oldAssembly, newAssembly, declarationDiffs, childrenDiffs)
        {
        }

        public override MetadataType MetadataType
        {
            get { return MetadataType.Assembly; }
        }

        protected override string GetElementShortName(AssemblyDefinition element)
        {
            return element.FullName;
        }
    }
}
