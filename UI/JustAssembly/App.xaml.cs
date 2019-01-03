using System;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using JustAssembly.MergeUtilities;
using JustAssembly.Infrastructure.Analytics;

namespace JustAssembly
{
    public partial class App : Application
    {
        private string[] args;

        public App()
        {
            Configuration.Analytics = AnalyticsServiceImporter.Instance.Import();
            
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            AssemblyHelper.ErrorReadingAssembly += OnErrorReadingAssembly;
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
            args = e.Args;
            base.OnStartup(e);
            
            Configuration.Analytics.Start();
            Configuration.Analytics.TrackFeature("Mode.UI");
            
            this.OnShellRun();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            
            Configuration.Analytics.Stop();
        }

        private void OnShellRun()
        {
            var bootstrapper = new JustAssemblyBootstrapper(args);

            bootstrapper.Run();
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Configuration.Analytics.TrackException(e.ExceptionObject as Exception);
            Configuration.Analytics.Stop();
        }
    }
}
