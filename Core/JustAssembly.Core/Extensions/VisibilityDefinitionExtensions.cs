using System;
using Mono.Cecil;

namespace JustAssembly.Core.Extensions
{
    static class VisibilityDefinitionExtensions
    {
        public static bool IsAPIDefinition(this IVisibilityDefinition self)
        {
            return self.IsPublic || self.IsFamilyOrAssembly || self.IsFamily;
        }
    }
}
