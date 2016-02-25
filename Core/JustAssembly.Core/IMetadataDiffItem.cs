using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace JustAssembly.Core
{
    public interface IMetadataDiffItem : IDiffItem
    {
        IEnumerable<IDiffItem> DeclarationDiffs { get; }
        IEnumerable<IMetadataDiffItem> ChildrenDiffs { get; }

        MetadataType MetadataType { get; }

        uint OldTokenID { get; }
        uint NewTokenID { get; }
    }

    public interface IMetadataDiffItem<T> : IMetadataDiffItem where T : class, IMetadataTokenProvider
    {
        T OldElement { get; }
        T NewElement { get; }
    }
}
