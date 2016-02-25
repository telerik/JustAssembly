using JustAssembly.Infrastructure.CodeViewer;
using JustAssembly.Nodes;
using JustAssembly.DiffAlgorithm;
using JustAssembly.DiffAlgorithm.Models;

namespace JustAssembly.ViewModels
{
    class XamlDiffTabItem : CodeDiffTabItemBase<XamlResourceNode>
    {
        public XamlDiffTabItem(XamlResourceNode xamlResourceNode)
            : base(xamlResourceNode)
        {
        }

        public override void LoadContent()
        {
            this.IsIndeterminate = true;
            this.IsBusy = true;

            this.header = instance.Name;
            this.toolTip = instance.FullName;

            if (!string.IsNullOrWhiteSpace(instance.OldSource))
            {
                this.LeftSourceCode = new DecompiledSourceCode(instance.OldSource);
            }
            if (!string.IsNullOrWhiteSpace(instance.NewSource))
            {
                this.RightSourceCode = new DecompiledSourceCode(instance.NewSource);
            }
            this.ApplyDiff();
            this.IsBusy = false;
        }
    }
}