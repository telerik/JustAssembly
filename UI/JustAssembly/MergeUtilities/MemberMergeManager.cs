using System;
using System.Collections.Generic;
using System.Linq;
using JustAssembly.Extensions;
using JustAssembly.Interfaces;
using JustDecompile.External.JustAssembly;

namespace JustAssembly.MergeUtilities
{
    class MemberMergeManager : MergeManagerBase<TypeMetadata, MemberDefinitionMetadataBase>
    {
        public MemberMergeManager(IOldToNewTupleMap<TypeMetadata> typesMap)
            : base(typesMap) { }

        public override IEnumerable<IOldToNewTupleMap<MemberDefinitionMetadataBase>> GetMergedCollection()
        {
            IList<MemberDefinitionMetadataBase> leftTypes = GetSortedAssemblyTypes(tupleMap.OldType);

            IList<MemberDefinitionMetadataBase> rigthTypes = GetSortedAssemblyTypes(tupleMap.NewType);

            return leftTypes.Merge(rigthTypes, TypeDefinitionNameComparer);
        }

        private IList<MemberDefinitionMetadataBase> GetSortedAssemblyTypes(TypeMetadata typeMetadata)
        {
            if (typeMetadata == null)
            {
                return new List<MemberDefinitionMetadataBase>();
            }
            List<MemberDefinitionMetadataBase> members = Decompiler.GetTypeMembers(typeMetadata.AssemblyPath, typeMetadata.Module.TokenId, typeMetadata.TokenId, SupportedLanguage.CSharp)
                                                                         .Select(i => MemberSelector(typeMetadata, i))
                                                                         .ToList();
            members.Sort(TypeDefinitionNameComparer);

            return members;
        }

        private MemberDefinitionMetadataBase MemberSelector(TypeMetadata typeMetadata, Tuple<MemberType, uint> memberTuple)
        {
            if (memberTuple.Item1 == MemberType.Type)
            {
                return new TypeMetadata(typeMetadata, memberTuple.Item2);
            }
            else
            {
                return new MemberMetadata(typeMetadata, memberTuple.Item1, memberTuple.Item2);
            }
        }

        private int TypeDefinitionNameComparer(MemberDefinitionMetadataBase oldType, MemberDefinitionMetadataBase newType)
        {
            if (oldType.MemberType == newType.MemberType)
            {
                if (oldType.MemberType == MemberType.Type)
                {
                    return ((TypeMetadata)oldType).GetTypeFullName().CompareTo(((TypeMetadata)newType).GetTypeFullName());
                }
                else
                {
                    return ((MemberMetadata)oldType).GetSignature().CompareTo(((MemberMetadata)newType).GetSignature());
                }
            }
            return oldType.MemberType < newType.MemberType ? -1 : 1;
        }
    }
}