using System.Collections.Generic;
using System.IO;
using System.Linq;
using JustAssembly.Extensions;
using JustAssembly.Interfaces;

namespace JustAssembly.MergeUtilities
{
    class FolderMergeManager : MergeManagerBase<string, string>
    {
        public FolderMergeManager(IOldToNewTupleMap<string> oldToNewTupleMap)
            : base(oldToNewTupleMap) { }

        public override IEnumerable<IOldToNewTupleMap<string>> GetMergedCollection()
        {
            IEnumerable<string> oldFiles = GetSortedSubFolderNodes(tupleMap.OldType);

            IEnumerable<string> newFiles = GetSortedSubFolderNodes(tupleMap.NewType);

            return oldFiles.Merge(newFiles, FolderNameComparer);
        }

        private List<string> GetSortedSubFolderNodes(string targetFolder)
        {
            List<string> fileNames = new List<string>();

            if (!Directory.Exists(targetFolder))
            {
                return fileNames;
            }
            foreach (string fileName in Directory.EnumerateDirectories(targetFolder))
            {
                fileNames.Add(fileName);
            }
            fileNames.Sort(FolderNameComparer);

            return fileNames;
        }

        private int FolderNameComparer(string oldName, string newName)
        {
            bool isOldNameEmpty = string.IsNullOrWhiteSpace(oldName);

            bool isNewNameEmpty = string.IsNullOrWhiteSpace(newName);

            if (!isOldNameEmpty && !isNewNameEmpty)
            {
                DirectoryInfo oldDir = new DirectoryInfo(oldName);

                DirectoryInfo newDir = new DirectoryInfo(newName);

                return string.Compare(oldDir.Name, newDir.Name, true);
            }
            else if (isOldNameEmpty && isNewNameEmpty)
            {
                return 0;
            }
            else if (isOldNameEmpty && !isNewNameEmpty)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}