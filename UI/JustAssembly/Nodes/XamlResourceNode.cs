using System.IO;
using JustAssembly.Interfaces;

namespace JustAssembly.Nodes
{
    class XamlResourceNode : DecompiledMemberNodeBase, IResourceNode
    {
        public XamlResourceNode(IOldToNewTupleMap<string> resourceMap, string name, ItemNodeBase parent, FilterSettings filterSettings)
            : base(name, parent, null, filterSettings)
        {
            this.ResourceMap = resourceMap;

            this.differenceDecoration = this.GetDifferenceDecoration();

            // This node has no child items, so this removes the expander.
            this.LazyLoading = false;
        }

        public override string FullName
        {
            get
            {
                return string.Format("{0}\\{1}", ParentNode.FullName, this.Name);
            }
        }

        public IOldToNewTupleMap<string> ResourceMap { get; private set; }

        public override object Icon
        {
            get
            {
                return ImagesResourceStrings.ResourceImage;
            }
        }

        public override NodeType NodeType
        {
            get
            {
                return NodeType.DecompiledResource;
            }
        }

        public override MemberDefinitionMetadataBase OldMemberMetada
        {
            get
            {
                return null;
            }
        }

        public override MemberDefinitionMetadataBase NewMemberMetada
        {
            get
            {
                return null;
            }
        }

        public override string OldSource
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ResourceMap.OldType))
                {
                    return File.ReadAllText(ResourceMap.OldType);
                }
                return string.Empty;
            }
        }

        public override string NewSource
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ResourceMap.NewType))
                {
                    return File.ReadAllText(ResourceMap.NewType);
                }
                return string.Empty;
            }
        }

        protected override DifferenceDecoration GetDifferenceDecoration()
        {
            if (CanUseParentDiffDecoration)
            {
                return this.ParentNode.DifferenceDecoration;
            }
            else if (string.IsNullOrWhiteSpace(ResourceMap.OldType))
            {
                return DifferenceDecoration.Added;
            }
            else if (string.IsNullOrWhiteSpace(ResourceMap.NewType))
            {
                return DifferenceDecoration.Deleted;
            }
            else
            {
                if (this.OldSource == this.NewSource)
                {
                    return DifferenceDecoration.NoDifferences;
                }
                else
                {
                    return DifferenceDecoration.Modified;
                }
            }
        }
    }
}