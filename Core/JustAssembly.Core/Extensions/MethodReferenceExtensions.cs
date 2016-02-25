using System;
using Mono.Cecil;

namespace JustAssembly.Core.Extensions
{
    static class MethodReferenceExtensions
    {
        public static string GetSignature(this MethodReference self)
        {
            string fullName = self.FullName;
            string returnTypeString = self.FixedReturnType.FullName;
            if (!fullName.StartsWith(returnTypeString))
            {
                throw new InvalidOperationException();
            }
            return fullName.Substring(returnTypeString.Length + 1);
        }

        public static string GetShortSignature(this MethodReference self)
        {
            string fullName = self.FullName;
            int index = fullName.IndexOf("::");
            if (index < 0)
            {
                throw new InvalidOperationException();
            }
            return fullName.Substring(index + 2);
        }
    }
}
