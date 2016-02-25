using System.IO;
using JustAssembly.Interfaces;
using JustAssembly.SelectorControl;

namespace JustAssembly.ViewModels
{
    class FolderComparisonViewModel : ComparisonSessionViewModelBase
    {
        public FolderComparisonViewModel()
            : base("Compare Folders")
        {
        }

        public override SelectedItemType SelectedItemType
        {
            get
            {
                return SelectedItemType.Folder;
            }
        }

        public override ITabSourceItem GetTabSourceItem()
        {
            return new FolderBrowseTabItem(this);
        }

        protected override bool GetLoadButtonState()
        {
            if (string.IsNullOrWhiteSpace(OldType) || string.IsNullOrWhiteSpace(NewType))
            {
                return false;
            }
            else if (!Directory.Exists(OldType) || !Directory.Exists(NewType))
            {
                return false;
            }
            return true;
        }
    }
}