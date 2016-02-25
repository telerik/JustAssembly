using System;
using System.Collections.Generic;

namespace JustAssembly.MergeUtilities
{
    public class ErrorAssemblyReadingEventArgs : EventArgs
    {
        public readonly IReadOnlyList<string> NotSupportedAssemblyPaths;

        public ErrorAssemblyReadingEventArgs(IReadOnlyList<string> notSupportedAssemblyPaths)
        {
            this.NotSupportedAssemblyPaths = notSupportedAssemblyPaths;
        }
    }
}