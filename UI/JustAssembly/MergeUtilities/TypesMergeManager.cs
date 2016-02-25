using System.Collections.Generic;
using System.Linq;
using JustAssembly.Extensions;
using JustAssembly.Interfaces;
using JustDecompile.External.JustAssembly;

namespace JustAssembly.MergeUtilities
{
    class TypesMergeManager : MergeManagerBase<ModuleMetadata, TypeMetadata>
    {
        public TypesMergeManager(IOldToNewTupleMap<ModuleMetadata> typesMap)
            : base(typesMap)
        {
        }

        public override IEnumerable<IOldToNewTupleMap<TypeMetadata>> GetMergedCollection()
        {
            IList<TypeMetadata> leftTypes = GetSortedByFullNameTypes(tupleMap.OldType);

            IList<TypeMetadata> rigthTypes = GetSortedByFullNameTypes(tupleMap.NewType);

            return leftTypes.Merge(rigthTypes, TypeDefinitionNameComparer);
        }

        private IList<TypeMetadata> GetSortedByFullNameTypes(ModuleMetadata moduleDefinitionToken)
        {
            if (moduleDefinitionToken == null)
            {
                return new List<TypeMetadata>();
            }
            List<TypeMetadata> types = Decompiler.GetModuleTypes(moduleDefinitionToken.AssemblyPath, moduleDefinitionToken.TokenId)
                                                       .Select(i => new TypeMetadata(moduleDefinitionToken, i))
                                                       .ToList();
            types.Sort(TypeDefinitionNameComparer);

            return types;
        }

        private int TypeDefinitionNameComparer(TypeMetadata oldType, TypeMetadata newType)
        {
            return oldType.GetTypeFullName().CompareTo(newType.GetTypeFullName());
        }
    }
}