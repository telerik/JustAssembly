using System;
using JustAssembly.Interfaces;
using JustAssembly.Nodes.APIDiff;
using JustAssembly.Nodes;

namespace JustAssembly.ViewModels
{
    class FolderBrowseTabItem : BrowserTabSourceItemBase
    {
        public FolderBrowseTabItem(IOldToNewTupleMap<string> tupleMap)
        {
            FilterSettings filterSettings = new FilterSettings(this.ShowAllUnmodified);
            this.nodes = new ItemNodeBase[] {
                new FolderNode(tupleMap, null, null, this, filterSettings, true)
            };

            this.header = GetTabTitle(tupleMap);
            this.toolTip = GetTabToolTip(tupleMap);
        }

        public override TabKind TabKind
        {
            get { return TabKind.DirectoryBrowse; }
        }

		public override void LoadContent()
		{
		}

        public override void ReloadContent()
        {
            this.nodes[currentNode].ReloadChildren();
        }

        private void OnFolderNodeChildrenLoaded(object sender, EventArgs e)
        {
        }

        public override void Dispose()
        {
            this.CancelProgress();
        }

        public override void OnProjectFileGenerated(JustDecompile.External.JustAssembly.IFileGeneratedInfo args)
        {
            this.Progress = this.progress + 1;
        }
    }
}