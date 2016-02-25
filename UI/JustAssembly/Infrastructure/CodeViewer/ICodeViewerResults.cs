using System;
using System.Collections.Generic;
using JustAssembly.DiffAlgorithm.Models;

namespace JustAssembly.Infrastructure.CodeViewer
{
    public interface ICodeViewerResults
    {
        Position GetMemberPosition();

        bool HighlighMember { get; }

        string GetSourceCode();

        void ApplyDiffInfo(DiffFile diffFile);

        ClassificationType GetLineDiffClassificationType(int lineNumber);

        int GetLinesCount();

        string GetLineNumberString(int lineNumber);
    }
}