using System;
using NUnit.Framework;

namespace AssemblyDiffTests.APIDiff
{
    [TestFixture]
    class ExistingAPIDiffTestFixture : BaseExistingAPIDiffTestFixture
    {
        [TestCase]
        public void SimpleAPIDiffTest()
        {
            RunTestCase("SimpleAPIDIffTest");
        }

        [TestCase]
        public void VariousAPIDiffsTest()
        {
            RunTestCase("VariousAPIDiffsTest");
        }
    }
}
