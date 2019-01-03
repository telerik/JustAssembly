using System;
using System.Collections.ObjectModel;
using System.Linq;
using JustAssembly.Interfaces;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.ObjectModel;

namespace JustAssembly.ViewModels
{
    class NewSessionViewModel : NotificationObject
    {
        private IComparisonSessionModel selectedSession;

        public NewSessionViewModel()
        {
            this.Tabs = new ObservableCollection<IComparisonSessionModel>
            {
                new AssembliesComparisonViewModel(),
                new FolderComparisonViewModel()
            };
            this.SelectedSession = Tabs[0];
        }

        public ObservableCollection<IComparisonSessionModel> Tabs { get; private set; }

        public IComparisonSessionModel SelectedSession
        {
            get
            {
                return this.selectedSession;
            }
            set
            {
                if (this.selectedSession != value)
                {
                    this.selectedSession = value;

                    this.RaisePropertyChanged("SelectedSession");
                }
            }
        }
    }
}
