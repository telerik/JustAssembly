using System;
using System.Collections.Generic;
using System.Linq;
using JustDecompile.External.JustAssembly;
using Mono.Cecil;

namespace JustAssembly.Core.DiffItems.Types
{
    class TypeDiffItem : BaseDiffItem<TypeDefinition>
    {
        public TypeDiffItem(TypeDefinition oldType, TypeDefinition newType, IEnumerable<IDiffItem> declarationDiffs, IEnumerable<IMetadataDiffItem> childrenDiffs)
            : base(oldType, newType, declarationDiffs, childrenDiffs)
        {
        }

        public override MetadataType MetadataType
        {
            get { return Core.MetadataType.Type; }
        }

        protected override string GetElementShortName(TypeDefinition typeDef)
        {
            return typeDef.Namespace + "." + Decompiler.GetTypeName(typeDef.Module.FilePath, typeDef.Module.MetadataToken.ToUInt32(), typeDef.MetadataToken.ToUInt32(), SupportedLanguage.CSharp);
        }
    }
}
