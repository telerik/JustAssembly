using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace JustAssembly.Core.DiffItems.Events
{
    class AddAccessorDiffItem : BaseMemberDiffItem<MethodDefinition>
    {
        public AddAccessorDiffItem(EventDefinition oldEvent, EventDefinition newEvent, IEnumerable<IDiffItem> declarationDiffs)
            : base(oldEvent.AddMethod, newEvent.AddMethod, declarationDiffs, null)
        {
        }

        public override MetadataType MetadataType
        {
            get { return Core.MetadataType.Method; }
        }

        protected override string GetElementShortName(MethodDefinition element)
        {
            return "add";
        }
    }
}
