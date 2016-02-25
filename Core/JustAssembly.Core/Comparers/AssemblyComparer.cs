using System;
using System.Collections.Generic;
using System.Linq;
using JustAssembly.Core.DiffItems;
using Mono.Cecil;
using JustAssembly.Core.Extensions;
using JustAssembly.Core.DiffItems.Assemblies;

namespace JustAssembly.Core.Comparers
{
    class AssemblyComparer
    {
        private readonly AssemblyDefinition oldAssembly;
        private readonly AssemblyDefinition newAssembly;

        public AssemblyComparer(AssemblyDefinition oldAssembly, AssemblyDefinition newAssembly)
        {
            this.oldAssembly = oldAssembly;
            this.newAssembly = newAssembly;
        }

        public IMetadataDiffItem<AssemblyDefinition> Compare()
        {
            IEnumerable<IDiffItem> declarationDiffs = new CustomAttributeComparer().GetMultipleDifferences(oldAssembly.CustomAttributes, newAssembly.CustomAttributes);
            IEnumerable<IDiffItem> childrenDiffs = new ModuleComparer().GetMultipleDifferences(oldAssembly.Modules, newAssembly.Modules);
            
            if (declarationDiffs.IsEmpty() && childrenDiffs.IsEmpty())
            {
                return null;
            }
            return new AssemblyDiffItem(oldAssembly, newAssembly, declarationDiffs, childrenDiffs.Cast<IMetadataDiffItem<ModuleDefinition>>());
        }
    }
}
