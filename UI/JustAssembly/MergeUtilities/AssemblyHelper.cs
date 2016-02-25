using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JustDecompile.External.JustAssembly;

namespace JustAssembly.MergeUtilities
{
    internal class AssemblyHelper
    {
        private readonly HashSet<string> notSupportedFiles;

        public AssemblyHelper()
        {
            this.notSupportedFiles = new HashSet<string>();
        }

        public void AddNotSupportedFiles(string fileName)
        {
            notSupportedFiles.Add(fileName);
        }

        public bool IsValidClrAssembly(string fileName)
        {
            return Decompiler.IsValidCLRAssembly(fileName);
        }

        public IReadOnlyList<string> GetNotSupportedFilesReadOnly()
        {
            return new ReadOnlyCollection<string>(notSupportedFiles.ToArray());
        }

        public void TriggerNotSupportedFilesEvent()
        {
            ErrorReadingAssembly(this, new ErrorAssemblyReadingEventArgs(GetNotSupportedFilesReadOnly()));
        }

        public static event EventHandler<ErrorAssemblyReadingEventArgs> ErrorReadingAssembly = delegate { };
    }
}
