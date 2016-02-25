using System;
using System.Linq;
using JustAssembly.Core;
using JustAssembly.Interfaces;
using JustAssembly.MergeUtilities;
using JustAssembly.Nodes;
using JustAssembly.Nodes.APIDiff;
using JustDecompile.External.JustAssembly;

namespace JustAssembly.ViewModels
{
    class AssemblyBrowseTabItem : BrowserTabSourceItemBase
    {
        private double verticalOffset;

        public AssemblyBrowseTabItem(IOldToNewTupleMap<string> comparableModel)
        {
            APIDiffInfo diffInfo = new APIDiffInfo(APIDiffHelper.GetAPIDifferences(comparableModel.OldType, comparableModel.NewType));

            var generationProjectInfoMap = new OldToNewTupleMap<GeneratedProjectOutputInfo>
               (
                   new GeneratedProjectOutputInfo(comparableModel.OldType),
                   new GeneratedProjectOutputInfo(comparableModel.NewType)
               );

            FilterSettings filterSettings = new FilterSettings(this.ShowAllUnmodified);
            this.nodes = new AssemblyNode[] {
                new AssemblyNode(comparableModel, null, null, this, filterSettings) { GenerationProjectInfoMap = generationProjectInfoMap }
            };

            this.header = GetTabTitle(comparableModel);
            this.toolTip = GetTabToolTip(comparableModel);

            this.nodes[0].ChildrenLoaded += OnAssemblyNodeChildrenLoaded;
            this.contentLoaded = new bool[2];
        }

        public override TabKind TabKind
        {
            get { return TabKind.AssemblyBrowse; }
        }

        public double VerticalOffset
        {
            get
            {
                return this.verticalOffset;
            }
            set
            {
                if (this.verticalOffset != value)
                {
                    this.verticalOffset = value;

                    this.RaisePropertyChanged("VerticalOffset");
                }
            }
        }

        public override void OnProjectFileGenerated(IFileGeneratedInfo args)
        {
            this.Progress = this.progress + 1;
        }

        public override void Dispose()
        {
            this.CancelProgress();

            this.RemoveFromCache(((AssemblyNode)this.nodes[0]).TypesMap.OldType);
            this.RemoveFromCache(((AssemblyNode)this.nodes[0]).TypesMap.NewType);
        }

        private bool RemoveFromCache(string assemblyName)
        {
            if (!string.IsNullOrWhiteSpace(assemblyName))
            {
                return GlobalDecompilationResultsRepository.Instance.RemoveByAssemblyPath(assemblyName);
            }
            return false;
        }

        private AssemblyNode GetCurrentNode()
        {
            return this.nodes[currentNode] as AssemblyNode;
        }

        public override void LoadContent()
        {
            var assemblyHelper = new AssemblyHelper();

            if (this.GetCurrentNode().HasInvalidAssemblies(assemblyHelper))
            {
                assemblyHelper.TriggerNotSupportedFilesEvent();
            }
        }

        private void OnAssemblyNodeChildrenLoaded(object sender, EventArgs e)
        {
            this.contentLoaded[this.currentNode] = true;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
