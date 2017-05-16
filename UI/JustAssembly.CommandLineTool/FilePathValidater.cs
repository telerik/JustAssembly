using System.Collections.Generic;
using System.IO;

namespace JustAssembly.CommandLineTool
{
    internal static class FilePathValidater
    {
        private static List<string> ValidInputFileExtensions = new List<string>() { ".dll", ".exe" };
        private static List<string> ValidOutputFileExtensions = new List<string>() { ".xml" };

        internal static bool ValidateInputFile(string filePath)
        {
            return ValidateFile(filePath, ValidInputFileExtensions) &&
                   File.Exists(filePath);
        }

        internal static bool ValidateOutputFile(string filePath)
        {
            return ValidateFile(filePath, ValidOutputFileExtensions);
        }

        private static bool ValidateFile(string filePath, List<string> validExtensions)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            string extension = Path.GetExtension(filePath);
            if (!validExtensions.Contains(extension))
            {
                return false;
            }

            return true;
        }
    }
}
