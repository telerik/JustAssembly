
using System;
using System.IO;
using JustAssembly.CommandLineTool;
using NUnit.Framework;

namespace JustAssembly.Tests
{
    [TestFixture]
    public class FileValidatorTests
    {
        [TestCase]
        public void ExtensionsMustBeValid()
        {
            var fullFilename = EnsureFileExists("SomeFile.xml");

            Assert.False(FilePathValidater.ValidateInputFile(fullFilename),"xml extension should not be accepted as valid");
        }


        [TestCase]
        public void HappyCase()
        {
            var fullFileName =EnsureFileExists("SomeFile.dll");
            Assert.True(FilePathValidater.ValidateInputFile(fullFileName),"dll should have been accepted");
        }

        [TestCase]
        public void HappyCase_RegardlessExtensionCasing()
        {
            var fullFileName =EnsureFileExists("SomeFile.DLL");
            Assert.True(FilePathValidater.ValidateInputFile(fullFileName),"dll should have been accepted");
        }




        private static string EnsureFileExists(string fileName)
        {
            var tempFolder = Path.GetTempPath();
            var folderName = Guid.NewGuid().ToString();
            var newFolder = Directory.CreateDirectory(folderName);
            var fullFilename = Path.Combine(newFolder.FullName, fileName);
            File.WriteAllText(fullFilename, string.Empty);
            return fullFilename;
        }
    }
}
