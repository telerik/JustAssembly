namespace JustAssembly.ViewModels
{
    public interface IShellViewModel
    {
        void CancelCurrentOperation();

        void OpenNewSessionCommandExecuted();

        void OpenNewSessionWithCmdLineArgsCommandExecuted();

    }
}