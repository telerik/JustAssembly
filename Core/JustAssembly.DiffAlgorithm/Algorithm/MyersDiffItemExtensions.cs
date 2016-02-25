using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustAssembly.DiffAlgorithm.Algorithm
{
    internal static class MyersDiffItemExtentions
    {
        public static void ProcessBlocks(this DiffItem diffItem,
            Action addModifiedBlock,
            Action addDeletedBlockWithImagineryPart,
            Action addInsertedBlockWithImagineryPart)
        {
            int i = 0;

            while (i < Math.Min(diffItem.DeletedA, diffItem.InsertedB))
            {
                addModifiedBlock();
                i++;
            }

            if (diffItem.DeletedA > diffItem.InsertedB)
            {
                while (i < diffItem.DeletedA)
                {
                    addDeletedBlockWithImagineryPart();
                    i++;
                }
            }
            else
            {
                while (i < diffItem.InsertedB)
                {
                    addInsertedBlockWithImagineryPart();
                    i++;
                }
            }
        }
    }
}
