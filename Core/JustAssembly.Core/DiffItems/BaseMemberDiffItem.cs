using System;
using System.Collections.Generic;
using JustAssembly.Core.Extensions;
using Mono.Cecil;

namespace JustAssembly.Core.DiffItems
{
    abstract class BaseMemberDiffItem<T> : BaseDiffItem<T> where T: class, IMemberDefinition
    {
        public BaseMemberDiffItem(T oldMember, T newMember, IEnumerable<IDiffItem> declarationDiffs, IEnumerable<IMetadataDiffItem> childrenDiffs)
            : base(oldMember, newMember, declarationDiffs, childrenDiffs)
        {
        }

        protected override string GetElementShortName(T element)
        {
            string type;
            string name;
            element.GetMemberTypeAndName(out type, out name);
            return name;
        }
    }
}
