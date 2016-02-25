using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JustAssembly.Core;
using JustAssembly.Interfaces;
using JustAssembly.Nodes.APIDiff;

namespace JustAssembly.Nodes
{
    internal class NamespaceNode : ItemNodeBase, IEquatable<NamespaceNode>
    {
        private readonly IList<IOldToNewTupleMap<TypeMetadata>> typesMap;
        private readonly IList<IMetadataDiffItem> diffItems;

        public NamespaceNode(string name, IList<IOldToNewTupleMap<TypeMetadata>> typesMap, IList<IMetadataDiffItem> diffItems, ItemNodeBase parent, FilterSettings filterSettings)
            : base(name, parent, filterSettings)
        {
            this.typesMap = typesMap;
            this.diffItems = diffItems;

            this.IsExpanded = true;
        }

        public override string FullName
        {
            get
            {
                return this.Name;
            }
        }

        public override object Icon
        {
            get { return ImagesResourceStrings.NamespaceNode; }
        }

        public override NodeType NodeType
        {
            get { return NodeType.Namespace; }
        }

        protected override void LoadChildren()
        {
            LoadAPIItemsContext context = this.diffItems != null ? new LoadAPIItemsContext(this.diffItems) : null;

			ObservableCollection<ItemNodeBase> result = new ObservableCollection<ItemNodeBase>(typesMap
                        .Select(tuple => GenerateTypeNode(tuple, context)));

            context.Validate();

            DispatcherObjectExt.Invoke(() =>
            {
                foreach (var item in result)
                {
                    this.Children.Add(item);
                }

                this.differenceDecoration = GetDifferenceDecoration();
            });
        }

        private TypeNode GenerateTypeNode(IOldToNewTupleMap<TypeMetadata> metadataTuple, LoadAPIItemsContext context)
        {
            return new TypeNode(metadataTuple, this, context.GenerateAPIDiffInfo(metadataTuple), this.FilterSettings);
        }

        public bool Equals(NamespaceNode other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Name == other.Name;
        }

        protected override DifferenceDecoration GetDifferenceDecoration()
        {
            if (this.CanUseParentDiffDecoration)
            {
                return ParentNode.DifferenceDecoration;
            }

            if (this.diffItems != null)
            {
                if (this.BreakingChangesOnly)
                {
                    return this.diffItems.Any(item => item != null && item.IsBreakingChange) ? DifferenceDecoration.Modified : DifferenceDecoration.NoDifferences;
                }
                return this.diffItems.Any(item => item != null) ? DifferenceDecoration.Modified : DifferenceDecoration.NoDifferences;
            }

            bool isNew = true;
            bool isDeleted = true;
            bool isModified = false;

            foreach (TypeNode typeMap in this.Children)
            {
                if (typeMap.TypesMap.OldType != null)
                {
                    isNew = false;
                }
                else
                {
                    isModified = true;
                }
                if (typeMap.TypesMap.NewType != null)
                {
                    isDeleted = false;
                }
                else
                {
                    isModified = true;
                }
                if (typeMap.DifferenceDecoration == DifferenceDecoration.Modified)
                {
                    return DifferenceDecoration.Modified;
                }
            }
            if (isNew)
            {
                return DifferenceDecoration.Added;
            }
            else if (isDeleted)
            {
                return DifferenceDecoration.Deleted;
            }
            else if (isModified)
            {
                return DifferenceDecoration.Modified;
            }
            else
            {
                return DifferenceDecoration.NoDifferences;
            }
        }
    }
}
