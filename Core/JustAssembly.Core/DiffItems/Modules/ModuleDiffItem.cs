using System;
using System.Collections.Generic;
using Mono.Cecil;
using System.Linq;

namespace JustAssembly.Core.DiffItems.Modules
{
    class ModuleDiffItem : BaseDiffItem<ModuleDefinition>
    {
        public ModuleDiffItem(ModuleDefinition oldModule, ModuleDefinition newModule, IEnumerable<IDiffItem> declarationDiffs, IEnumerable<IMetadataDiffItem<TypeDefinition>> diffChildren)
            : base(oldModule, newModule, declarationDiffs, diffChildren)
        {
        }

        public override MetadataType MetadataType
        {
            get { return Core.MetadataType.Module; }
        }

        protected override string GetElementShortName(ModuleDefinition element)
        {
            return element.Name;
        }
    }
}
