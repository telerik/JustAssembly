using JustAssembly.API.Analytics;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace JustAssembly
{
    static class Configuration
    {
        private const string AppDirectoryName = "JustAssembly";

        public static IAnalyticsService Analytics { get; set; }

        public static string GetApplicationTempFolder
        {
            get { return Path.Combine(Path.GetTempPath(), AppDirectoryName); }
        }

        public static string JustAssemblyAppDataFolder
        {
            get
            {
                string userRoamingHomeDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string justAssemblyFolder = Path.Combine(userRoamingHomeDir, AppDirectoryName);
                EnsureDirectoryExists(justAssemblyFolder);

                return justAssemblyFolder;
            }
            
        }

        public static void ShowExceptionMessage(Exception exception)
        {
            ShowExceptionMessage(new ReadOnlyCollection<Exception>(new[] { exception }));
        }

        public static void ShowExceptionMessage(ReadOnlyCollection<Exception> readOnlyCollection)
        {
            var exceptionStringBuilder = new StringBuilder(readOnlyCollection.Count * 3);

            foreach (Exception exception in readOnlyCollection)
            {
                exceptionStringBuilder.Append(exception.Message)
                                      .AppendLine()
                                      .AppendLine(exception.StackTrace);
            }
            ToolWindow.ShowDialog(new ErrorMessageWindow(exceptionStringBuilder.ToString(), "Exception"), width: 800, height: 500);
        }

        public readonly static Color AddedColor = Color.FromRgb(221, 255, 221);
        public readonly static Color DeletedColor = Color.FromRgb(255, 221, 221);
        public readonly static Color ModifiedColor = Color.FromRgb(240, 240, 255);

        private static void EnsureDirectoryExists(string directoryName)
        {
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
        }
    }
}
