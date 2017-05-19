using JustAssembly.API.Analytics;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;

namespace JustAssembly.Infrastructure.Analytics
{
    public class AnalyticsServiceImporter
    {
        private static AnalyticsServiceImporter instance;

        [Import]
        private IAnalyticsService analyticsService;

        private AnalyticsServiceImporter()
        {
        }

        public static AnalyticsServiceImporter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AnalyticsServiceImporter();
                }

                return instance;
            }
        }

        public IAnalyticsService Import()
        {
            try
            {
                AggregateCatalog catalog = new AggregateCatalog();
                catalog.Catalogs.Add(new AssemblyCatalog(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "JustAssembly.Analytics.dll")));

                CompositionContainer container = new CompositionContainer(catalog);
                container.ComposeParts(this);
            }
            catch (Exception) { }

            if (analyticsService != null)
            {
                return analyticsService;
            }
            else
            {
                return new EmptyAnalyticsService();
            }
        }
    }
}
