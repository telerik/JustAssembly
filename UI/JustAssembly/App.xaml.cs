using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using JustAssembly.MergeUtilities;
using ICSharpCode.TreeView;
using System.IO;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using JustAssembly.API.Analytics;
using JustAssembly.Infrastructure.Analytics;

namespace JustAssembly
{
    public partial class App : Application
    {
        [Import]
        private IAnalyticsService analyticsService;

        public App()
        {
            ImportAnalyticsService();
            
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            AssemblyHelper.ErrorReadingAssembly += OnErrorReadingAssembly;
        }

        private void ImportAnalyticsService()
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
                Configuration.Analytics = analyticsService;
            }
            else
            {
                Configuration.Analytics = new EmptyAnalyticsService();
            }
        }

        private void OnErrorReadingAssembly(object sender, ErrorAssemblyReadingEventArgs e)
        {
            if (e.NotSupportedAssemblyPaths.Count == 0)
            {
                return;
            }
            StringBuilder unsupportedFilesNames = new StringBuilder();
            foreach (string assembly in e.NotSupportedAssemblyPaths)
            {
                unsupportedFilesNames.Append(assembly);
                unsupportedFilesNames.Append(Environment.NewLine);
            }
            string errorCaption = "Not supported file(s):";

            string errorMessage = unsupportedFilesNames.ToString();

            DispatcherObjectExt.BeginInvoke(() => ToolWindow.ShowDialog(new ErrorMessageWindow(errorMessage, errorCaption), width: 800, height: 500), DispatcherPriority.Background);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            Configuration.Analytics.Start();
            
            this.OnShellRun();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            
            Configuration.Analytics.Stop();
        }

        private void OnShellRun()
        {
            var bootstrapper = new JustAssemblyBootstrapper();

            bootstrapper.Run();
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Configuration.Analytics.TrackException(e.ExceptionObject as Exception);
            Configuration.Analytics.Stop();
        }
    }
}
