using System;

namespace JustAssembly.Core
{
    public interface IDiffItem
    {
        DiffType DiffType { get; }

        string ToXml();

        bool IsBreakingChange { get; }
    }
}
