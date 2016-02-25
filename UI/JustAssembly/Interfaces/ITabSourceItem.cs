using System;
using System.Linq;

namespace JustAssembly.Interfaces
{
    interface ITabSourceItem : IProgressNotifier
    {
        TabKind TabKind { get; }

        string Header { get; }

        string ToolTip { get; }

        void Dispose();

        void LoadContent();

		void ReloadContent();
    }
}
