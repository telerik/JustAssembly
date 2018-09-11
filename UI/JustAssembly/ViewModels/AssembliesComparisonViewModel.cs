using System.IO;
using JustAssembly.Interfaces;
using JustAssembly.SelectorControl;

namespace JustAssembly.ViewModels
{
    class AssembliesComparisonViewModel : ComparisonSessionViewModelBase
    {
        public AssembliesComparisonViewModel(string[] args)
            : base("Compare Assemblies")
        {
            if (args != null && args.Length == 2)
            {
                OldType = args[0];
                NewType = args[1];
            }
        }

        public AssembliesComparisonViewModel()
            : base("Compare Assemblies")
        {
        }

        public override SelectedItemType SelectedItemType
        {
            get
            {
                return SelectedItemType.File;
            }
        }

        public override ITabSourceItem GetTabSourceItem()
        {
            return new AssemblyBrowseTabItem(this);
        }

        protected override bool GetLoadButtonState()
        {
            if (string.IsNullOrWhiteSpace(OldType) || string.IsNullOrWhiteSpace(NewType))
            {
                return false;
            }
            else if (!File.Exists(OldType) || !File.Exists(NewType))
            {
                return false;
            }
            return true;
        }
    }
}