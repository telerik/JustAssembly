using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace JustAssembly.Core.DiffItems.Events
{
    class EventDiffItem : BaseMemberDiffItem<EventDefinition>
    {
        public EventDiffItem(EventDefinition oldEvent, EventDefinition newEvent, IEnumerable<IDiffItem> declarationDiffs, IEnumerable<IMetadataDiffItem> childrenDiffs)
            :base(oldEvent, newEvent, declarationDiffs, childrenDiffs)
        {
        }

        public override MetadataType MetadataType
        {
            get { return Core.MetadataType.Event; }
        }
    }
}
