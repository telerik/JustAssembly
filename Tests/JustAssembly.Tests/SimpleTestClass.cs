using System;
using System.Linq;
using Mono.Cecil;
using NUnit.Framework;

namespace JustAssembly.Tests
{
    [TestFixture]
    public class SimpleTestClass
    {
        [TestCase]
        public void SimpleMonoCecilTest()
        {
            AssemblyDefinition thisAssembly = AssemblyDefinition.ReadAssembly("JustAssembly.Tests.dll", new ReaderParameters(GlobalAssemblyResolver.Instance));
            Assert.IsTrue(thisAssembly.MainModule.Types.Any(type => type.Name == "SimpleTestClass"));
        }
    }
}
