using System;
using System.Collections.ObjectModel;
using JustAssembly.Interfaces;
using JustAssembly.Nodes.APIDiff;
using Microsoft.Practices.Prism.ViewModel;
using ICSharpCode.TreeView;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace JustAssembly.Nodes
{
    abstract class ItemNodeBase : SharpTreeNode
    {
        private bool breakingChangesOnly;

        protected readonly APIDiffInfo apiDiffInfo;

        protected DifferenceDecoration differenceDecoration = DifferenceDecoration.NoDifferences;
		
        public readonly ItemNodeBase ParentNode;

        public ItemNodeBase(string name, FilterSettings filterSettings)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this.Name = name;
            this.LazyLoading = true;
            this.Children = new ObservableCollection<ItemNodeBase>();
            this.Children.CollectionChanged += OnChildrenCollectionChanged;
            this.FilterSettings = filterSettings;

            this.RaisePropertyChanged("DifferenceDecoration");
        }

        public ItemNodeBase(string name, ItemNodeBase parentNode, FilterSettings filterSettings)
            : this(name, filterSettings)
        {
            this.ParentNode = parentNode;
        }

        public ItemNodeBase(string name, ItemNodeBase parentNode, APIDiffInfo apiDiffInfo, FilterSettings filterSettings)
            : this(name, parentNode, filterSettings)
        {
            this.apiDiffInfo = apiDiffInfo;
        }

        public abstract NodeType NodeType { get; }

        public IOldToNewTupleMap<GeneratedProjectOutputInfo> GenerationProjectInfoMap { get; set; }

        public string Name { get; private set; }

        public abstract string FullName { get; }

        public DifferenceDecoration DifferenceDecoration
        {
            get { return this.differenceDecoration; }
        }

        public override object Text
        {
            get
            {
                return this.Name;
            }
        }

        protected bool CanUseParentDiffDecoration
        {
            get
            {
                return ParentNode != null && 
                      (ParentNode.DifferenceDecoration == DifferenceDecoration.Added || 
                       ParentNode.DifferenceDecoration == DifferenceDecoration.Deleted);
            }
        }

        public FilterSettings FilterSettings { get; set; }

        public new ObservableCollection<ItemNodeBase> Children { get; private set; }

        public virtual void ReloadChildren()
        {
            base.Children.Clear();
            foreach (ItemNodeBase child in this.Children)
            {
                if (child.ShouldBeShown(this.FilterSettings))
                {
                    base.Children.Add(child);
                    child.ReloadChildren();
                }
            }
        }

        protected virtual void OnChildrenLoaded()
        {
            ChildrenLoaded(this, EventArgs.Empty);
        }

        protected abstract DifferenceDecoration GetDifferenceDecoration();

        protected bool ApiOnlyFilter(IOldToNewTupleMap<MemberMetadataBase> tuple)
        {
            return this.apiDiffInfo == null || tuple.OldType != null && tuple.OldType.IsPublic || tuple.NewType != null && tuple.NewType.IsPublic;
        }

        public event EventHandler ChildrenLoaded = delegate { };

        public bool BreakingChangesOnly
        {
            get
            {
                if (this.ParentNode != null)
                {
                    this.breakingChangesOnly = this.ParentNode.BreakingChangesOnly;
                }
                return this.breakingChangesOnly;
            }
            set
            {
                this.breakingChangesOnly = value;
            }
        }

        public void RefreshDecoration()
        {
            DifferenceDecoration old = this.differenceDecoration;
            foreach (ItemNodeBase item in this.Children)
            {
                item.RefreshDecoration();
            }

            this.differenceDecoration = GetDifferenceDecoration();
            if (this.differenceDecoration != old)
            {
                this.RaisePropertyChanged("DifferenceDecoration");
            }
        }

        public bool ShouldBeShown(FilterSettings settings)
        {
            if (settings.ShowUnmodified)
            {
                return true;
            }
            else if (this.DifferenceDecoration != DifferenceDecoration.NoDifferences)
            {
                return true;
            }

            return false;
        }

        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ItemNodeBase child in e.NewItems)
                {
                    if (child.ShouldBeShown(this.FilterSettings))
                    {
                        base.Children.Add(child);
                    }
                }
            }
        }
    }
}
