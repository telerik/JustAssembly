using System;
using System.Linq;
using JustDecompile.External.JustAssembly;
using Mono.Cecil;

namespace JustAssembly.Core.Extensions
{
    static class MemberDefinitionExtensions
    {
        private const string Separator = " : ";

        public static void GetMemberTypeAndName(this IMemberDefinition self, out string type, out string name)
        {
            TypeDefinition declaringType = self.DeclaringType;
            ModuleDefinition module = declaringType.Module;
            string assemblyFilePath = module.Assembly.MainModule.FilePath;
            string nameWithType =
                Decompiler.GetMemberName(assemblyFilePath, module.MetadataToken.ToUInt32(), declaringType.MetadataToken.ToUInt32(), self.MetadataToken.ToUInt32(), SupportedLanguage.CSharp);

            int index = nameWithType.IndexOf(Separator);
            if (index == -1)
            {
                // The member is constructor, hense it has no type.
                name = nameWithType;
                type = null;
            }
            else
            {
                name = nameWithType.Substring(0, index);
                type = nameWithType.Substring(index + Separator.Length);
            }
        }
    }
}
