using System;
using System.Linq;

namespace JustAssembly.Core.DiffItems.Common
{
    class StaticFlagChangedDiffItem : BaseDiffItem
    {
        private readonly bool isNewMemberStatic;

        public StaticFlagChangedDiffItem(bool isNewMemberStatic)
            :base(DiffType.Modified)
        {
            this.isNewMemberStatic = isNewMemberStatic;
        }

        protected override string GetXmlInfoString()
        {
            return string.Format("Member changed to {0}.", isNewMemberStatic ? "static" : "instance");
        }

        public override bool IsBreakingChange
        {
            get { return true; }
        }
    }
}
