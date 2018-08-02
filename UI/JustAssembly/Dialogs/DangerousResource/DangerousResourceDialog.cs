using System.Windows;
using JustAssembly.Core;

namespace JustAssembly.Dialogs.DangerousResource
{
    internal class DangerousResourceDialog
    {
        private readonly string assemblyFileName;
        private readonly AssemblyType assemblyType;

        public DangerousResourceDialog(string assemblyFileName, AssemblyType assemblyType)
        {
            this.assemblyFileName = assemblyFileName;
            this.assemblyType = assemblyType;
        }

        public DangerousResourceDialogResult Show()
        {
            string title = "Warning";
            string message = this.GetMessage();
            DangerousResourceDialogResult dialogResult;

            MessageBoxResult result = MessageBox.Show(Application.Current.MainWindow, message, title, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                dialogResult = DangerousResourceDialogResult.Yes;
            }
            else
            {
                dialogResult = DangerousResourceDialogResult.No;
            }
            
            this.TrackDangerousResourceDecompilationUserChoice(dialogResult);

            return dialogResult;
        }

        private string GetMessage()
        {
            return $"\"{this.assemblyFileName}\" [{this.assemblyType.ToString()}] contains resources that may contain malicious code. Decompilation of such resources will result in execution of that malicious code. Do you want to decompile those resources and use them for the assembly comparison?";
        }

        private void TrackDangerousResourceDecompilationUserChoice(DangerousResourceDialogResult dialogResult)
        {
            string featureToReport = "DangerousResourcesDecompilationDialogResult" + '.' + dialogResult.ToString();

            Configuration.Analytics.TrackFeature(featureToReport);
        }
    }
}
