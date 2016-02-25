using System.Threading;
using JustDecompile.External.JustAssembly;

namespace JustAssembly.Interfaces
{
    interface IProgressNotifier : IFileGenerationNotifier
    {
        bool IsBusy { get; set; }

        bool IsIndeterminate { get; set; }

        string LoadingMessage { get; set; }

        int Progress { get; set; }

        void Completed();

        void CancelProgress();

        CancellationToken GetCanellationToken();
    }
}