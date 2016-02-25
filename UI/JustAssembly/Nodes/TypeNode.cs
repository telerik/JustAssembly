using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using JustAssembly.Interfaces;
using JustAssembly.MergeUtilities;
using JustAssembly.Nodes.APIDiff;
using JustDecompile.External.JustAssembly;

namespace JustAssembly.Nodes
{
    class TypeNode : DecompiledMemberNodeBase
    {
        public TypeNode(string name, ItemNodeBase parent, APIDiffInfo apiDiffInfo, FilterSettings filterSettings)
            : base(name, parent, apiDiffInfo, filterSettings) { }

        public TypeNode(IOldToNewTupleMap<TypeMetadata> typesMap, ItemNodeBase parent, APIDiffInfo apiDiffInfo, FilterSettings filterSettings)
            : base(GetMemberName(typesMap), parent, apiDiffInfo, filterSettings)
        {
            this.TypesMap = typesMap;

            TypeMetadata oldTypeMetadata = TypesMap.OldType;
            TypeMetadata newTypeMetadata = TypesMap.NewType;

            IDecompilationResults oldResult;
            IDecompilationResults newResult;

            if (oldTypeMetadata != null &&
                GlobalDecompilationResultsRepository.Instance.TryGetDecompilationResult(oldTypeMetadata.AssemblyPath, oldTypeMetadata.Module.TokenId, oldTypeMetadata.TokenId, out oldResult))
            {
                this.OldDecompileResult = oldResult;
            }
            if (newTypeMetadata != null &&
                GlobalDecompilationResultsRepository.Instance.TryGetDecompilationResult(newTypeMetadata.AssemblyPath, newTypeMetadata.Module.TokenId, newTypeMetadata.TokenId, out newResult))
            {
                this.NewDecompileResult = newResult;
            }
            this.differenceDecoration = GetDifferenceDecoration();
        }

        public IOldToNewTupleMap<TypeMetadata> TypesMap { get; private set; }

        public override string FullName
        {
            get
            {
                return string.Format("{0}.{1}", ParentNode.FullName, this.Name);
            }
        }

        public override MemberDefinitionMetadataBase OldMemberMetada
        {
            get { return TypesMap.OldType; }
        }

        public override MemberDefinitionMetadataBase NewMemberMetada
        {
            get { return TypesMap.NewType; }
        }

        public override NodeType NodeType
        {
            get { return NodeType.TypeDefinition; }
        }

        public override object Icon
        {
            get { return ImagesResourceStrings.TypeNode; }
        }

        protected override void LoadChildren()
        {
            LoadAPIItemsContext context = this.apiDiffInfo.GenerateLoadAPIItemsContext();

            var memberManager = new MemberMergeManager(TypesMap);
            ObservableCollection<ItemNodeBase> collection = new ObservableCollection<ItemNodeBase>(memberManager
                                                                                                    .GetMergedCollection()
                                                                                                    .Where(ApiOnlyFilter)
                                                                                                    .Select(tuple => GetItemNodeFromMemberType(tuple, context)));

            context.Validate();

            DispatcherObjectExt.BeginInvoke(() =>
                 {
                     foreach (var item in collection)
                     {
                         this.Children.Add(item);
                     }
                     this.OnChildrenLoaded();

                 }, DispatcherPriority.Background);
        }

        private string CleanExceptionSource(IDecompilationResults decompilationResult, string sourceCode)
        {
            var spansForRemoving = new List<IOffsetSpan>();

            foreach (uint memberId in decompilationResult.MembersWithExceptions)
            {
                IOffsetSpan memberOffset;

                if (decompilationResult.MemberTokenToDecompiledCodeMap.TryGetValue(memberId, out memberOffset))
                {
                    spansForRemoving.Add(memberOffset);
                }
            }
            spansForRemoving.Sort((i1, i2) => i2.StartOffset.CompareTo(i1.StartOffset));

            for (int i = 0; i < spansForRemoving.Count; i++)
            {
                IOffsetSpan memberOffset = spansForRemoving[i];

                sourceCode = sourceCode.Remove(memberOffset.StartOffset, memberOffset.EndOffset - memberOffset.StartOffset + 1);
            }
            return sourceCode;
        }

        private DecompiledMemberNodeBase GetItemNodeFromMemberType(IOldToNewTupleMap<MemberDefinitionMetadataBase> membersMap, LoadAPIItemsContext context)
        {
            MemberDefinitionMetadataBase metadata = membersMap.GetFirstNotNullItem();

            APIDiffInfo diffInfo = context.GenerateAPIDiffInfo(membersMap);

            if (metadata.MemberType == MemberType.Type)
            {
                return new NestedTypeNode((new OldToNewTupleMap<TypeMetadata>((TypeMetadata)membersMap.OldType, (TypeMetadata)membersMap.NewType)), this, diffInfo, this.FilterSettings);
            }
            else
            {
                return new MemberNode((new OldToNewTupleMap<MemberMetadata>((MemberMetadata)membersMap.OldType, (MemberMetadata)membersMap.NewType)), this, diffInfo, this.FilterSettings);
            }
        }

        protected static string GetMemberName(IOldToNewTupleMap<TypeMetadata> typesMap)
        {
            return typesMap.GetFirstNotNullItem().GetName();
        }

        protected override DifferenceDecoration GetDifferenceDecoration()
        {
            if (this.CanUseParentDiffDecoration)
            {
                return this.ParentNode.DifferenceDecoration;
            }

            if (this.apiDiffInfo != null)
            {
                return this.apiDiffInfo.APIDiffItem.GetDifferenceDecoration(this.BreakingChangesOnly);
            }

            if (this.TypesMap.OldType == null)
            {
                return DifferenceDecoration.Added;
            }
            else if (this.TypesMap.NewType == null)
            {
                return DifferenceDecoration.Deleted;
            }
            else if (OldDecompileResult == null && NewDecompileResult == null)
            {
                return DifferenceDecoration.NoDifferences;
            }
            else
            {
                string oldCleanSource = CleanExceptionSource(OldDecompileResult, this.OldSource);

                string newCleanSource = CleanExceptionSource(NewDecompileResult, this.NewSource);

                return oldCleanSource == newCleanSource ? DifferenceDecoration.NoDifferences : DifferenceDecoration.Modified;
            }
        }

        private class NestedTypeNode : TypeNode
        {
            public NestedTypeNode(IOldToNewTupleMap<TypeMetadata> typesMap, ItemNodeBase parent, APIDiffInfo apiDiffInfo, FilterSettings filterSettings)
                : base(GetMemberName(typesMap), parent, apiDiffInfo, filterSettings)
            {
                this.TypesMap = typesMap;
            }

            protected override DifferenceDecoration GetDifferenceDecoration()
            {
                if (this.CanUseParentDiffDecoration)
                {
                    return this.ParentNode.DifferenceDecoration;
                }

                if (this.apiDiffInfo != null)
                {
                    return this.apiDiffInfo.APIDiffItem.GetDifferenceDecoration(this.BreakingChangesOnly);
                }

                if (this.TypesMap.OldType == null)
                {
                    return DifferenceDecoration.Added;
                }
                else if (this.TypesMap.NewType == null)
                {
                    return DifferenceDecoration.Deleted;
                }
                else if (this.ParentNode.DifferenceDecoration == DifferenceDecoration.Modified)
                {
                    if (this.OldDecompileResult.MemberTokenToDecompiledCodeMap.ContainsKey(TypesMap.OldType.TokenId))
                    {
                        return this.GetMemberSource(this.OldDecompileResult, this.TypesMap.OldType) == this.GetMemberSource(this.NewDecompileResult, this.TypesMap.NewType) ?
                            DifferenceDecoration.NoDifferences : DifferenceDecoration.Modified;
                    }
                }
                return DifferenceDecoration.NoDifferences;
            }
        }
    }
}
