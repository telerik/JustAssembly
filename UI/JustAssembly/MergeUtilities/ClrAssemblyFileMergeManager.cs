using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JustAssembly.Extensions;
using JustAssembly.Interfaces;
using JustDecompile.External.JustAssembly;

namespace JustAssembly.MergeUtilities
{
    class ClrAssemblyFileMergeManager : MergeManagerBase<string, string>
    {
        private static readonly HashSet<string> supportedFileExtensions = new HashSet<string> { ".dll", ".exe", ".winmd" };

        private readonly bool provideNotSupportedAssemblyNotification;
        private readonly IProgressNotifier progressNotifier;

        public ClrAssemblyFileMergeManager(IOldToNewTupleMap<string> tupleMap, bool provideNotSupportedAssemblyNotification, IProgressNotifier progressNotifier)
            : base(tupleMap)
        {
            this.progressNotifier = progressNotifier;
            this.provideNotSupportedAssemblyNotification = provideNotSupportedAssemblyNotification;
        }

        public int GetAssembliesCount(IOldToNewTupleMap<string> folderMap)
        {
            int result = 0;

            string root = folderMap.OldType;
            foreach (var extension in supportedFileExtensions)
            {
                result += Directory.GetFiles(root, "*" + extension, SearchOption.AllDirectories).Length;
            }

            root = folderMap.NewType;
            foreach (var extension in supportedFileExtensions)
            {
                result += Directory.GetFiles(root, "*" + extension, SearchOption.AllDirectories).Length;
            }

            return result;
        }

        public override IEnumerable<IOldToNewTupleMap<string>> GetMergedCollection()
        {
            AssemblyHelper assemblyHelper = new AssemblyHelper();

            List<string> oldFiles = this.GetSortedClrAssemblyNodes(tupleMap.OldType, assemblyHelper);

            List<string> newFiles = this.GetSortedClrAssemblyNodes(tupleMap.NewType, assemblyHelper);

            if (provideNotSupportedAssemblyNotification)
            {
                assemblyHelper.TriggerNotSupportedFilesEvent();
            }
            return oldFiles.Merge(newFiles, FileNameComparer);
        }

        private List<string> GetSortedClrAssemblyNodes(string targetFolder, AssemblyHelper assemblyHelper)
        {
            List<string> clrAssemblyPath = new List<string>();

            if (!Directory.Exists(targetFolder))
            {
                return clrAssemblyPath;
            }
            foreach (string fileName in Directory.EnumerateFiles(targetFolder))
            {
                if (supportedFileExtensions.Contains(Path.GetExtension(fileName).ToLower()))
                {
                    if (assemblyHelper.IsValidClrAssembly(fileName))
                    {
                        clrAssemblyPath.Add(fileName);
                    }
                    else if (provideNotSupportedAssemblyNotification)
                    {
                        assemblyHelper.AddNotSupportedFiles(fileName);
                    }
                    IFileGeneratedInfo args = new FileGeneratedInfo(fileName, false);
                    progressNotifier.OnProjectFileGenerated(args);
                }
            }
            clrAssemblyPath.Sort(FileNameComparer);

            return clrAssemblyPath;
        }

        private int FileNameComparer(string oldName, string newName)
        {
            bool isOldNameNull = oldName == null;

            bool isNewNameNull = newName == null;

            if (!isOldNameNull && !isNewNameNull)
            {
                return Path.GetFileName(oldName).CompareTo(Path.GetFileName(newName));
            }
            else if (isOldNameNull && isNewNameNull)
            {
                return 0;
            }
            else if (isOldNameNull && !isNewNameNull)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
    class FileGeneratedInfo : IFileGeneratedInfo
    {
        private readonly string fullPath;
        private readonly bool hasErrors;

        public FileGeneratedInfo(string fullPath, bool hasErrors)
        {
            this.fullPath = fullPath;
            this.hasErrors = hasErrors;
        }

        public string FullPath
        {
            get
            {
                return this.fullPath;
            }
        }

        public bool HasErrors
        {
            get
            {
                return this.hasErrors;
            }
        }
    }
}
