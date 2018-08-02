namespace JustAssembly.Dialogs.DangerousResource
{
    internal class DangerousResourceDialogWithAnalyticsTracking : DangerousResourceDialog
    {
        public DangerousResourceDialogWithAnalyticsTracking(string assemblyFileName, AssemblyType assemblyType)
            :base(assemblyFileName, assemblyType) { }

        public override DangerousResourceDialogResult Show()
        {
            DangerousResourceDialogResult dialogResult = base.Show();

            this.TrackDangerousResourceDecompilationUserChoice(dialogResult);

            return dialogResult;
        }

        private void TrackDangerousResourceDecompilationUserChoice(DangerousResourceDialogResult dialogResult)
        {
            string featureToReport = "DangerousResourcesDecompilationDialogResult" + '.' + dialogResult.ToString();

            Configuration.Analytics.TrackFeature(featureToReport);
        }
    }
}
