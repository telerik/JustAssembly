using System;

namespace JustAssembly.Core.DiffItems.Methods
{
    class VirtualFlagChangedDiffItem : BaseDiffItem
    {
        private readonly bool isNewMethodVirtual;

        public VirtualFlagChangedDiffItem(bool isNewMethodVirtual)
            :base(DiffType.Modified)
        {
            this.isNewMethodVirtual = isNewMethodVirtual;
        }

        protected override string GetXmlInfoString()
        {
            return string.Format("Method changed to {0}virtual.", this.isNewMethodVirtual ? string.Empty : "non-");
        }

        public override bool IsBreakingChange
        {
            get { return !this.isNewMethodVirtual; }
        }
    }
}
