using System;
using Mono.Cecil;

namespace JustAssembly.Core.Comparers
{
    static class VisibilityComparer
    {
        internal static int CompareTypes(TypeDefinition first, TypeDefinition second)
        {
            return GetVisibility(second) - GetVisibility(first);
        }

        private static Visibility GetVisibility(TypeDefinition type)
        {
            if (type.IsPublic || type.IsNestedPublic)
            {
                return Visibility.Public;
            }

            if (type.IsNestedFamily || type.IsNestedFamilyOrAssembly)
            {
                return Visibility.Protected;
            }

            if (type.IsNotPublic || type.IsNestedAssembly || type.IsNestedFamilyAndAssembly)
            {
                return Visibility.Internal;
            }

            return Visibility.Private;
        }

        internal static int CompareVisibilityDefinitions(IVisibilityDefinition first, IVisibilityDefinition second)
        {
            return (int)GetVisibility(second) - (int)GetVisibility(first);
        }

        private static Visibility GetVisibility(IVisibilityDefinition visibilityDef)
        {
            if (visibilityDef.IsPublic)
            {
                return Visibility.Public;
            }

            if (visibilityDef.IsFamily || visibilityDef.IsFamilyOrAssembly)
            {
                return Visibility.Protected;
            }

            if (visibilityDef.IsFamilyAndAssembly || visibilityDef.IsAssembly)
            {
                return Visibility.Internal;
            }

            return Visibility.Private;
        }
    }

    enum Visibility : byte
    {
        //Order matters for comparison
        Private,
        Internal,
        Protected,
        Public,
    }
}
