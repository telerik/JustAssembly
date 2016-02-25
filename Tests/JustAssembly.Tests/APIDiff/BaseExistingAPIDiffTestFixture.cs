using System;
using System.IO;
using JustAssembly.Core;
using NUnit.Framework;

namespace AssemblyDiffTests.APIDiff
{
    abstract class BaseExistingAPIDiffTestFixture
    {
        protected string AssembliesDirectory
        {
            get
            {
                return @"..\..\Assemblies\APIDiff";
            }
        }

        protected void RunTestCase(string testName)
        {
            string extendedPath = Path.Combine(AssembliesDirectory, testName, testName);
            string oldFile = extendedPath + ".old.dll";
            string newFile = extendedPath + ".dll";
            string expectedResultFile = extendedPath + ".xml";

            string expected = GetExpectedResult(expectedResultFile);
            string result = GetAPIDiffResult(oldFile, newFile);

            Assert.AreEqual(expected, result);
        }

        private string GetExpectedResult(string expectedResultFile)
        {
            using (StreamReader sr = new StreamReader(expectedResultFile))
            {
                return sr.ReadToEnd();
            }
        }

        private string GetAPIDiffResult(string oldFile, string newFile)
        {
            IDiffItem diffItem = APIDiffHelper.GetAPIDifferences(oldFile, newFile);
            return diffItem == null ? string.Empty : diffItem.ToXml();
        }
    }
}
