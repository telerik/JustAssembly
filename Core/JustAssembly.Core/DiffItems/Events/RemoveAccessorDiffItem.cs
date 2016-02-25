using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace JustAssembly.Core.DiffItems.Events
{
    class RemoveAccessorDiffItem : BaseMemberDiffItem<MethodDefinition>
    {
        public RemoveAccessorDiffItem(EventDefinition oldEvent, EventDefinition newEvent, IEnumerable<IDiffItem> declarationDiffs)
            : base(oldEvent.RemoveMethod, newEvent.RemoveMethod, declarationDiffs, null)
        {
        }

        public override MetadataType MetadataType
        {
            get { return Core.MetadataType.Method; }
        }

        protected override string GetElementShortName(MethodDefinition element)
        {
            return "remove";
        }
    }
}
