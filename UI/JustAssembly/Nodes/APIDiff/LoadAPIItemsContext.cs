using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JustAssembly.Core;
using JustAssembly.Interfaces;

namespace JustAssembly.Nodes.APIDiff
{
    class LoadAPIItemsContext
    {
        private readonly Dictionary<uint, IMetadataDiffItem> oldChildrenTokens = new Dictionary<uint, IMetadataDiffItem>();
        private readonly Dictionary<uint, IMetadataDiffItem> newChildrenTokens = new Dictionary<uint, IMetadataDiffItem>();

        public bool IsEmpty
        {
            get
            {
                return oldChildrenTokens.Count == 0 && newChildrenTokens.Count == 0;
            }
        }

        public LoadAPIItemsContext(APIDiffInfo info)
        {
            if (info.APIDiffItem != null)
            {
                PopulateSets(info.APIDiffItem.ChildrenDiffs);
            }
        }

        public LoadAPIItemsContext(IEnumerable<IMetadataDiffItem> diffItems)
        {
            PopulateSets(diffItems);
        }

        private void PopulateSets(IEnumerable<IMetadataDiffItem> diffItems)
        {
            foreach (IMetadataDiffItem diffItem in diffItems)
            {
                if (diffItem == null)
                {
                    continue;
                }

                if (diffItem.DiffType != DiffType.Deleted)
                {
                    this.newChildrenTokens.Add(diffItem.NewTokenID, diffItem);
                }
                if (diffItem.DiffType != DiffType.New)
                {
                    this.oldChildrenTokens.Add(diffItem.OldTokenID, diffItem);
                }
            }
        }

        public IMetadataDiffItem GetDiffItem(IOldToNewTupleMap<MemberMetadataBase> tuple)
        {
            IMetadataDiffItem oldDiffItem = null;
            IMetadataDiffItem newDiffItem = null;

            if (tuple.OldType == null && tuple.NewType == null)
            {
                throw new ArgumentException();
            }

            if (tuple.OldType != null)
            {
                if (!oldChildrenTokens.TryGetValue(tuple.OldType.TokenId, out oldDiffItem))
                {
                    oldDiffItem = null;
                }
                else
                {
                    oldChildrenTokens.Remove(tuple.OldType.TokenId);
                }
            }

            if (tuple.NewType != null)
            {
                if (!newChildrenTokens.TryGetValue(tuple.NewType.TokenId, out newDiffItem))
                {
                    newDiffItem = null;
                }
                else
                {
                    newChildrenTokens.Remove(tuple.NewType.TokenId);
                }
            }

            if (oldDiffItem != null && newDiffItem != null && oldDiffItem != newDiffItem)
            {
                throw new Exception("Diff item mismatch");
            }

            return oldDiffItem ?? newDiffItem;
        }
    }
}
