using System;
using System.IO;
using JustDecompile.External.JustAssembly;

namespace JustAssembly.Nodes
{
    class GeneratedProjectOutputInfo
    {
        private readonly string outputPath;

        public GeneratedProjectOutputInfo(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }
            do
            {
                this.outputPath = GenerateOutputFolder(fileName);

            } while (Directory.Exists(outputPath));

			Directory.CreateDirectory(this.outputPath);

            this.Timestamp = DateTime.Now;
        }

        public string OutputPath
        {
            get
            {
                return this.outputPath;
            }
        }

        public readonly DateTime Timestamp;

        public string GetRelativePath(string absolutePath)
        {
            return absolutePath.Remove(0, OutputPath.Length + 1);
        }

        private string GenerateOutputFolder(string fileName)
        {
            fileName = string.Format("{0}\\{1}_{2}",
                                    Configuration.GetApplicationTempFolder,
                                    Path.GetFileNameWithoutExtension(fileName),
                                    Path.GetRandomFileName());
            return fileName;
        }
    }
}