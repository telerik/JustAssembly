using System;
using System.Collections.Generic;
using System.Linq;
using JustAssembly.Extensions;
using JustAssembly.Interfaces;

namespace JustAssembly.MergeUtilities
{
    class ResourceMergeManager : MergeManagerBase<ICollection<string>, string>
    {
		private readonly Comparison<string> oldResourceNameComparer;
		private readonly Comparison<string> newResourceNameComparer;
        private readonly Comparison<string> resourceNameComparer;

        public ResourceMergeManager(IOldToNewTupleMap<ICollection<string>> resourcesMap, Comparison<string> oldResourceNameComparer, Comparison<string> newResourceNameComparer, 
			Comparison<string> resourceNameComparer)
            : base(resourcesMap)
        {
			this.oldResourceNameComparer = oldResourceNameComparer;
			this.newResourceNameComparer = newResourceNameComparer;
            this.resourceNameComparer = resourceNameComparer;
        }

        public override IEnumerable<IOldToNewTupleMap<string>> GetMergedCollection()
        {
            List<string> oldResourcesList = this.tupleMap.OldType.ToList();
            oldResourcesList.Sort(oldResourceNameComparer);

            List<string> newResourcesList = this.tupleMap.NewType.ToList();
            newResourcesList.Sort(newResourceNameComparer);

            return oldResourcesList.Merge(newResourcesList, resourceNameComparer);
        }
    }
}
