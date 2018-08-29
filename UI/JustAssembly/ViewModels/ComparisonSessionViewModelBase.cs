using System.Windows.Input;
using JustAssembly.Interfaces;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using JustAssembly.SelectorControl;

namespace JustAssembly.ViewModels
{
    abstract class ComparisonSessionViewModelBase : NotificationObject, IOldToNewTupleMap<string>, IComparisonSessionModel
    {
        private string oldPath;
        private string newPath;

        public ICommand SwapPathsCommand { get; private set; }

        public ComparisonSessionViewModelBase(string header, string[] args)
        {
            if (args != null && args.Length == 2)
            {
                oldPath = args[0];
                newPath = args[1];
            }

            this.SwapPathsCommand = new DelegateCommand(OnSwapPathsCommandExecuted);

            this.Header = header;
        }

        public abstract SelectedItemType SelectedItemType { get; }

        public string Header { get; private set; }

        public string OldType
        {
            get
            {
                return this.oldPath;
            }
            set
            {
                if (this.oldPath != value)
                {
                    this.oldPath = value;

                    this.RaisePropertyChanged("OldType");

                    this.RaisePropertyChanged("IsLoadEnabled");
                }
            }
        }

        public string NewType
        {
            get
            {
                return this.newPath;
            }
            set
            {
                if (this.newPath != value)
                {
                    this.newPath = value;

                    this.RaisePropertyChanged("NewType");

                    this.RaisePropertyChanged("IsLoadEnabled");
                }
            }
        }

        public bool IsLoadEnabled
        {
            get { return GetLoadButtonState(); }
        }

        protected abstract bool GetLoadButtonState();

        public abstract ITabSourceItem GetTabSourceItem();

        private void OnSwapPathsCommandExecuted()
        {
            string tempPath = this.OldType;

            this.OldType = this.NewType;

            this.NewType = tempPath;
        }

        public string GetFirstNotNullItem()
        {
            return OldType ?? NewType;
        }
    }
}