using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JustAssembly.DiffAlgorithm.Models;

namespace JustAssembly.DiffAlgorithm.Algorithm
{
    internal static class DiffFileExtentions
    {
        public static void AddDiffBlock(this DiffFile file, DiffInfo textInfo, DiffBlockType diffBlockType)
        {
            file.AppendDiffBlock(textInfo.BlockPosition, textInfo.TextOffset,
                diffBlockType);
            textInfo.BlockPosition++;
        }

        public static void AddImaginaryBlock(this DiffFile file, DiffInfo textInfo)
        {
            file.AppendDiffBlock(textInfo.BlockPosition, textInfo.TextOffset,
                DiffBlockType.Imaginary);
            textInfo.BlockPosition++;
            textInfo.TextOffset++;
        }

        public static void AddUnchangedBlocks(this DiffFile diffFile, DiffInfo textInfo)
        {
            while (textInfo.BlockPosition - textInfo.TextOffset < textInfo.HighBound)
            {
                diffFile.AppendDiffBlock(textInfo.BlockPosition,
                    textInfo.TextOffset,
                    DiffBlockType.Unchanged);
                textInfo.BlockPosition++;
            }
        }

        public static void AppendDiffBlock(this DiffFile diffFile, int blockPosition, int offset,
            DiffBlockType type)
        {
            var diffBlock = diffFile.Blocks.LastOrDefault();

            if (diffBlock != null && diffBlock.Type == type &&
                diffBlock.EndPosition + 1 == blockPosition)
            {
                diffBlock.EndPosition++;
                return;
            }

            diffBlock = new DiffBlock(offset, blockPosition, blockPosition, type);
            diffFile.Blocks.Add(diffBlock);
        }
    }
}
