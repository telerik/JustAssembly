using System;
using JustAssembly.Infrastructure.CodeViewer;
using JustAssembly.Nodes;

namespace JustAssembly.ViewModels
{
    class JustAssemblyTabItem : CodeDiffTabItemBase<DecompiledMemberNodeBase>
    {
        public JustAssemblyTabItem(DecompiledMemberNodeBase typeNode)
            : base(typeNode) { }

        public override void LoadContent()
        {
            this.IsIndeterminate = true;
            this.IsBusy = true;

            this.header = instance.Name;
            this.toolTip = instance.FullName;

            if (this.instance.OldDecompileResult != null)
            {
                this.LeftSourceCode = new DecompiledSourceCode(instance.OldMemberMetada, instance.OldDecompileResult, instance.OldSource);
            }
            if (this.instance.NewDecompileResult != null)
            {
                this.RightSourceCode = new DecompiledSourceCode(instance.NewMemberMetada, instance.NewDecompileResult, instance.NewSource);
            }
            this.ApplyDiff();
            this.IsBusy = false;
        }
    }
}