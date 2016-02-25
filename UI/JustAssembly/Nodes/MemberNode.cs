using System;
using System.Linq;
using JustAssembly.Interfaces;
using JustAssembly.Nodes.APIDiff;
using JustDecompile.External.JustAssembly;

namespace JustAssembly.Nodes
{
    class MemberNode : DecompiledMemberNodeBase
    {
        private readonly IOldToNewTupleMap<MemberMetadata> membersMap;

        public MemberNode(IOldToNewTupleMap<MemberMetadata> membersMap, ItemNodeBase parent, APIDiffInfo apiDiffInfo, FilterSettings filterSettings)
            : base(membersMap.GetFirstNotNullItem().GetSignature(), parent, apiDiffInfo, filterSettings)
        {
            this.membersMap = membersMap;

            this.differenceDecoration = GetDifferenceDecoration();

            // This node has no child items, so this removes the expander.
            this.LazyLoading = false;
        }

        public override object Icon
        {
            get
            {
                MemberMetadata member = membersMap.GetFirstNotNullItem();

                switch (member.MemberType)
                {
                    case MemberType.Event:
                        return ImagesResourceStrings.EventNode;

                    case MemberType.Field:
                        return ImagesResourceStrings.FieldNode;

                    case MemberType.Method:
                        return ImagesResourceStrings.MethodNode;

                    case MemberType.Property:
                        return ImagesResourceStrings.PropertyNode;

                    default:
                        return ImagesResourceStrings.TypeNode;
                }
            }
        }

        public override NodeType NodeType
        {
            get { return NodeType.MemberDefinition; }
        }

        public override string FullName
        {
            get { return string.Format("{0}.{1}", ParentNode.FullName, this.Name); }
        }

        public override MemberDefinitionMetadataBase OldMemberMetada
        {
            get { return this.membersMap.OldType; }
        }

        public override MemberDefinitionMetadataBase NewMemberMetada
        {
            get { return this.membersMap.NewType; }
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

            
            if (this.membersMap.OldType == null)
            {
                return DifferenceDecoration.Added;
            }
            else if (this.membersMap.NewType == null)
            {
                return DifferenceDecoration.Deleted;
            }
            else if (this.ParentNode.DifferenceDecoration == DifferenceDecoration.Modified)
            {
                IOffsetSpan offsetSpanL = null;
                IOffsetSpan offsetSpanR = null;

                bool containsValues = OldDecompileResult.MemberTokenToDecompiledCodeMap.TryGetValue(membersMap.OldType.TokenId, out offsetSpanL) && 
                                      NewDecompileResult.MemberTokenToDecompiledCodeMap.TryGetValue(membersMap.NewType.TokenId, out offsetSpanR);

                if (containsValues)
                {
                    return this.GetMemberSource(this.OldDecompileResult, membersMap.OldType) == this.GetMemberSource(this.NewDecompileResult, membersMap.NewType) ?
                        DifferenceDecoration.NoDifferences : DifferenceDecoration.Modified;
                }
            }
            return DifferenceDecoration.NoDifferences;
        }
    }
}
