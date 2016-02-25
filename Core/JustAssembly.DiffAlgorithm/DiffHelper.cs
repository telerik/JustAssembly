using System;
using System.Collections.Generic;
using System.Linq;
using JustAssembly.DiffAlgorithm.Algorithm;
using JustAssembly.DiffAlgorithm.Models;

namespace JustAssembly.DiffAlgorithm
{
    public static class DiffHelper
    {
        private static readonly string[] lineSeparators = new string[] { "\r\n", "\n", "\r" };

        public static DiffResult[] Diff(IEnumerable<string> firstContents, IEnumerable<string> secondContents)
        {
            return firstContents.Zip(secondContents, (firstContent, secondContent) => Diff(firstContent, secondContent)).ToArray();
        }

        public static DiffResult Diff(string firstFile, string secondFile)
        {
            return new DiffText(
                SplitLines(firstFile),
                SplitLines(secondFile),
                MyersDiff.Instance).GetChanges();
        }

        public static string[] SplitLines(string fileContent)
        {
            if (fileContent == null)
            {
                return new string[0];
            }

            return fileContent.Split(lineSeparators, StringSplitOptions.None);
        }
    }
}
