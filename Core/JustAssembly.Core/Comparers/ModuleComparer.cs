using System;
using System.Collections.Generic;
using System.Linq;
using JustAssembly.Core.DiffItems;
using JustAssembly.Core.DiffItems.Modules;
using JustAssembly.Core.Extensions;
using Mono.Cecil;

namespace JustAssembly.Core.Comparers
{
    class ModuleComparer : BaseDiffComparer<ModuleDefinition>
    {
        protected override IDiffItem GetMissingDiffItem(ModuleDefinition element)
        {
            return new ModuleDiffItem(element, null, null, null);
        }

        protected override IDiffItem GenerateDiffItem(ModuleDefinition oldElement, ModuleDefinition newElement)
        {
            IEnumerable<IDiffItem> declarationDiffs = EnumerableExtensions.ConcatAll<IDiffItem>(GetCustomAttributeDiffs(oldElement, newElement), GetReferenceDiffs(oldElement, newElement));
            IEnumerable<IDiffItem> childrenDiffs = GetTypeDiffs(oldElement, newElement);
            
            if(declarationDiffs.IsEmpty() && childrenDiffs.IsEmpty())
            {
                return null;
            }
            return new ModuleDiffItem(oldElement, newElement, declarationDiffs, childrenDiffs.Cast<IMetadataDiffItem<TypeDefinition>>());
        }

        private IEnumerable<IDiffItem> GetCustomAttributeDiffs(ModuleDefinition oldModule, ModuleDefinition newModule)
        {
            return new CustomAttributeComparer().GetMultipleDifferences(oldModule.CustomAttributes, newModule.CustomAttributes);
        }

        private IEnumerable<IDiffItem> GetReferenceDiffs(ModuleDefinition oldModule, ModuleDefinition newModule)
        {
            return new ReferenceComparer().GetMultipleDifferences(oldModule.AssemblyReferences, newModule.AssemblyReferences);
        }

        private IEnumerable<IDiffItem> GetTypeDiffs(ModuleDefinition oldModule, ModuleDefinition newModule)
        {
            return new TypeComparer().GetMultipleDifferences(oldModule.Types, newModule.Types);
        }

        protected override IDiffItem GetNewDiffItem(ModuleDefinition element)
        {
            return new ModuleDiffItem(null, element, null, null);
        }

        protected override int CompareElements(ModuleDefinition x, ModuleDefinition y)
        {
            return x.Name.CompareTo(y.Name);
        }

        protected override bool IsAPIElement(ModuleDefinition element)
        {
            return element.Types.Any(type => type.IsPublic);
        }
    }
}
