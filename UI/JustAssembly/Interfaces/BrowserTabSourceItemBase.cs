using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using JustAssembly.Extensions;
using JustAssembly.Infrastructure;
using JustAssembly.Nodes;
using ICSharpCode.TreeView;

namespace JustAssembly.Interfaces
{
    abstract class BrowserTabSourceItemBase : TabSourceItemBase
    {
        private int projectCount;
        private bool breakingChangesOnly;
        private bool showAllUnmodified;
        private string selectedModificationFilter;

        protected string toolTip;
        protected ItemNodeBase[] nodes;
        protected bool[] contentLoaded;
        protected bool apiOnly;
        protected int currentNode;

        public BrowserTabSourceItemBase()
        {
            this.JustAssemblyViewModel = new JustAssemblyViewModel();

            this.JustAssemblyViewModel.PropertyChanged += OnJustAssemblyPropertyChanged;

            this.RaisePropertyChanged("JustAssemblyViewModel");
            
            this.ShowAllUnmodified = false;
        }

        public JustAssemblyViewModel JustAssemblyViewModel { get; private set; }

        public int ProjectCount
        {
            get
            {
                return this.projectCount;
            }
            set
            {
                if (this.projectCount != value)
                {
                    this.projectCount = value;
                }
                this.RaisePropertyChanged("LoadingString");
            }
        }

        public bool ShowAllUnmodified
        {
            get
            {
                return this.showAllUnmodified;
            }
            set
            {
                if (showAllUnmodified != value)
                {
                    if (this.Root != null)
                    {
                        this.Root.FilterSettings.ShowUnmodified = value;
                        this.Root.ReloadChildren();
                    }

                    this.showAllUnmodified = value;

                    this.RaisePropertyChanged("ShowAllUnmodified");
                }
            }
        }
        
        public bool APIOnly
        {
            get
            {
                return apiOnly;
            }
            set
            {
                if (apiOnly != value)
                {
                    apiOnly = value;
                    currentNode = value ? 1 : 0;
                    if (!contentLoaded[currentNode])
                    {
                        ReloadContent();
                    }
                    this.RaisePropertyChanged("APIOnly");
                }
            }
        }

        public bool BreakingChangesOnly
        {
            get
            {
                return breakingChangesOnly;
            }
            set
            {
                if (breakingChangesOnly != value)
                {
                    breakingChangesOnly = value;
                    ItemNodeBase apiNode = this.nodes[1];
                    apiNode.BreakingChangesOnly = value;
                    apiNode.RefreshDecoration();
                    this.RaisePropertyChanged("BreakingChangesOnly");
                }
            }
        }
        
        public override void ReloadContent()
        {
            this.nodes[this.currentNode].ReloadChildren();
        }

        public ItemNodeBase Root
        {
            get
            {
                if (nodes != null)
                {
                    return nodes[currentNode];
                }

                return null;
            }
        }

        public override string ToolTip
        {
            get
            {
                return toolTip;
            }
        }

        protected string GetTabTitle(IOldToNewTupleMap<string> comparableModel)
        {
            string oldItemName = string.Empty;
            string newItemName = string.Empty;
            if (comparableModel.OldType != null)
            {
                var oldDirInfo = new DirectoryInfo(comparableModel.OldType);
                oldItemName = oldDirInfo.Name;
            }
            if (comparableModel.NewType != null)
            {
                var newDirInfo = new DirectoryInfo(comparableModel.NewType);
                newItemName = newDirInfo.Name;
            }
            return string.Format("{0} <> {1}", oldItemName, newItemName);
        }

        protected string GetTabToolTip(IOldToNewTupleMap<string> comparableModel)
        {
            string oldItemName = string.Empty;
            string newItemName = string.Empty;
            if (comparableModel.OldType != null)
            {
                var oldDirInfo = new DirectoryInfo(comparableModel.OldType);
                oldItemName = oldDirInfo.FullName;
            }
            if (comparableModel.NewType != null)
            {
                var newDirInfo = new DirectoryInfo(comparableModel.NewType);
                newItemName = newDirInfo.FullName;
            }
            return string.Format("{0} <> {1}", oldItemName, newItemName);
        }

        private void OnJustAssemblyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedJustAssembly")
            {
                this.UpdateJustAssembly();
            }
        }

        private void UpdateJustAssembly()
        {
            switch (JustAssemblyViewModel.GetSelectedJustAssembly())
            {
                case JustAssemblyerences.All:
                    this.APIOnly = false;
                    this.BreakingChangesOnly = false;
                    break;
                case JustAssemblyerences.AllPublicApi:
                    this.APIOnly = true;
                    this.BreakingChangesOnly = false;
                    break;
                case JustAssemblyerences.PublicApiBreakingChanges:
                    this.APIOnly = true;
                    this.BreakingChangesOnly = true;
                    break;
            }
        }
    }
}