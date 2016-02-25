using System;
using System.Collections.Generic;
using JustAssembly.DiffAlgorithm.Models;

namespace JustAssembly.DiffAlgorithm.Algorithm
{
    public class DiffText
    {

        private IDiffAlgorithm diff;
        private IList<string> oldLines;
        private IList<string> newLines;

        public DiffText(IList<string> oldLines, IList<string> newLines, IDiffAlgorithm diffAlgorithm)
        {
            this.diff = diffAlgorithm;
            this.oldLines = oldLines;
            this.newLines = newLines;
        }

        public DiffResult GetChanges()
        {
            var diffItems = this.diff.DiffText(oldLines, newLines);
            return this.GetChangesCore(diffItems);
        }

        private DiffResult GetChangesCore(IEnumerable<DiffItem> diffItems)
        {
            var oldFile = new DiffFile();
            var newFile = new DiffFile();
            oldFile.Lines = oldLines;
            newFile.Lines = newLines;

            var oldTextInfo = new DiffInfo();
            var newTextInfo = new DiffInfo();
            foreach (DiffItem diffItem in diffItems)
            {
                oldTextInfo.HighBound = diffItem.StartA;
                newTextInfo.HighBound = diffItem.StartB;

                oldFile.AddUnchangedBlocks(oldTextInfo);
                newFile.AddUnchangedBlocks(newTextInfo);

                diffItem.ProcessBlocks(
                    () =>
                    {
                        this.AddModifiedBlock(oldTextInfo, oldFile, newTextInfo, newFile);
                    },
                    () =>
                    {
                        AddBlockWithImagineryPart(newTextInfo, newFile, oldTextInfo, oldFile, DiffBlockType.Deleted);
                    },
                    () =>
                    {
                        AddBlockWithImagineryPart(oldTextInfo, oldFile, newTextInfo, newFile, DiffBlockType.Inserted);
                    });
            }

            oldTextInfo.HighBound = this.oldLines.Count;
            newTextInfo.HighBound = this.newLines.Count;

            oldFile.AddUnchangedBlocks(oldTextInfo);
            newFile.AddUnchangedBlocks(newTextInfo);

            return new DiffResult(oldFile, newFile);
        }

        private static void AddBlockWithImagineryPart(DiffInfo imagineryTextInfo, DiffFile imagineryBlockFile,
            DiffInfo textInfo, DiffFile file, DiffBlockType nonImagineryBlockType)
        {
            imagineryBlockFile.AddImaginaryBlock(imagineryTextInfo);

            file.AddDiffBlock(textInfo, nonImagineryBlockType);
        }

        private void AddModifiedBlock(DiffInfo oldTextInfo, DiffFile oldFile,
            DiffInfo newTextInfo, DiffFile newFile)
        {
            oldFile.AddDiffBlock(oldTextInfo, DiffBlockType.Modified);

            newFile.AddDiffBlock(newTextInfo, DiffBlockType.Modified);
        }
    }
}
