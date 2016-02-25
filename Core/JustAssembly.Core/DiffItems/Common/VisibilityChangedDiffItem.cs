using System;
using System.Collections.Generic;
using System.Linq;

namespace JustAssembly.Core.DiffItems.Common
{
    class VisibilityChangedDiffItem : BaseDiffItem
    {
        private readonly bool reduced;

        public VisibilityChangedDiffItem(bool reduced)
            :base(DiffType.Modified)
        {
            this.reduced = reduced;
        }

        protected override string GetXmlInfoString()
        {
            return string.Format("Member is {0} visible.", this.reduced ? "less" : "more");
        }

        public override bool IsBreakingChange
        {
            get { return this.reduced; }
        }
    }
}
