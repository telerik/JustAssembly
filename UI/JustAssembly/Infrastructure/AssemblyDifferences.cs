using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.ViewModel;

namespace JustAssembly.Infrastructure
{
    class JustAssemblyViewModel : NotificationObject
    {
        private string selectedJustAssembly;

        private readonly IDictionary<string, JustAssemblyerences> diffMap;

        public JustAssemblyViewModel()
        {
            this.diffMap = new Dictionary<string, JustAssemblyerences>
            {
                {"All kinds of API changes", JustAssemblyerences.All},
                {"Public API without version changes", JustAssemblyerences.AllPublicApi},
                {"Public API breaking changes", JustAssemblyerences.PublicApiBreakingChanges},
            };
            this.SelectedJustAssembly = this.JustAssemblys.First();
        }

        public string SelectedJustAssembly
        {
            get
            {
                return this.selectedJustAssembly;
            }
            set
            {
                if (selectedJustAssembly != value)
                {
                    selectedJustAssembly = value;

                    this.RaisePropertyChanged("SelectedJustAssembly");
                }
            }
        }

        public IEnumerable<string> JustAssemblys
        {
            get { return diffMap.Keys; }
        }

        public JustAssemblyerences GetSelectedJustAssembly()
        {
            return diffMap[SelectedJustAssembly];
        }
    }

    enum JustAssemblyerences : int
    {
        All,
        AllPublicApi,
        PublicApiBreakingChanges
    }
}
