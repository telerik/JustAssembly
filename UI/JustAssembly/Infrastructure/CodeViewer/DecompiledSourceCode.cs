using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JustAssembly.Extensions;
using JustAssembly.DiffAlgorithm.Models;
using JustDecompile.External.JustAssembly;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using System.Globalization;

namespace JustAssembly.Infrastructure.CodeViewer
{
    class DecompiledSourceCode : ICodeViewerResults
    {
        private string sourceCode;
        private readonly IDecompilationResults decompilationResult;
        private readonly MemberDefinitionMetadataBase memberMetadata;
        private readonly List<DiffLineInfo> originalLineNumberToRelativeDiffLineOffsetMap;
        private readonly Dictionary<int, ClassificationType> lineToClasificationTypeMap;
        private int totalLineCount;

        public DecompiledSourceCode(string sourceCode)
        {
            this.originalLineNumberToRelativeDiffLineOffsetMap = new List<DiffLineInfo>();

            this.sourceCode = sourceCode;

            this.lineToClasificationTypeMap = new Dictionary<int, ClassificationType>();
        }

        public DecompiledSourceCode(MemberDefinitionMetadataBase memberNode, IDecompilationResults decompilationResult, string sourceCode)
            : this(sourceCode)
        {
            this.memberMetadata = memberNode;

            this.decompilationResult = decompilationResult;
        }

        private string NewLine
        {
            get { return this.decompilationResult != null ? this.decompilationResult.CodeViewerResults.NewLine : "\n"; }
        }

        public IBackgroundRenderer BackgroundRenderer { get; set; }

        public bool HighlighMember
        {
            get
            {
                if (memberMetadata == null)
                {
                    return false;
                }
                else if (memberMetadata.MemberType != MemberType.Type)
                {
                    return true;
                }
                else if (memberMetadata.Type != null)
                {
                    return true;
                }
                return false;
            }
        }

        public override string ToString()
        {
            return sourceCode;
        }

        public string GetSourceCode()
        {
            return sourceCode;
        }

        public void ApplyDiffInfo(DiffFile diffFile)
        {
            StringBuilder diffCodeBuilder = new StringBuilder();
            int currentLine = 0;
            int diffLineOffset = 0;
            for (int i = 0; i < diffFile.Blocks.Count; i++)
            {
                DiffBlock block = diffFile.Blocks[i];

                for (; currentLine < block.StartPosition - block.Offset; currentLine++)
                {
                    string line = diffFile.Lines[currentLine];
                    diffCodeBuilder.Append(line).Append(this.NewLine);
                }
                if (block.Type == DiffBlockType.Imaginary)
                {
                    int addedLines = block.EndPosition - block.StartPosition + 1;
                    for (int j = addedLines, k = 0; j > 0; j--, k++)
                    {
                        diffCodeBuilder.Append(this.NewLine);
                    }

                    diffLineOffset += addedLines;
                    originalLineNumberToRelativeDiffLineOffsetMap.Add(new DiffLineInfo(currentLine, diffLineOffset));
                }
                if (block.Type != DiffBlockType.Unchanged)
                {
                    for (int k = block.StartPosition; k <= block.EndPosition; k++)
                    {
                        lineToClasificationTypeMap.Add(k, ConvertDiffBlockTypeToClassificationType(block.Type));
                    }
                }
            }
            for (; currentLine < diffFile.Lines.Count; currentLine++)
            {
                diffCodeBuilder.Append(diffFile.Lines[currentLine]).Append(this.NewLine);
            }
            this.totalLineCount = currentLine + diffLineOffset;
            this.sourceCode = diffCodeBuilder.ToString();
            this.BackgroundRenderer = new DiffBackgroundRenderer(lineToClasificationTypeMap);
        }

        public Position GetMemberPosition()
        {
            if (memberMetadata == null)
            {
                return Position.Empty;
            }
            int lineNumber = memberMetadata.MemberType == MemberType.Type ?
                                    decompilationResult.CodeViewerResults.GetLineFromTypeToken(memberMetadata.TokenId) :
                                    decompilationResult.CodeViewerResults.GetLineFromMemberToken(memberMetadata.Type.TokenId, memberMetadata.TokenId);

            if (decompilationResult.MemberDeclarationToCodePostionMap.ContainsKey(memberMetadata.TokenId))
            {
                IOffsetSpan offsetSpan = decompilationResult.MemberDeclarationToCodePostionMap[memberMetadata.TokenId];

                int offset = this.TransformOriginalLineToDiffLineOffset(lineNumber);

                return new Position(offsetSpan.StartOffset + offset, offsetSpan.EndOffset - offsetSpan.StartOffset + 1, lineNumber + offset);
            }
            return Position.Empty;
        }

        private int TransformOriginalLineToDiffLineOffset(int originalLine)
        {
            int index = this.originalLineNumberToRelativeDiffLineOffsetMap.BinarySearch(new DiffLineInfo(originalLine, 0), new DiffLineInfoOriginalComparer());
            if (index < 0)
            {
                index = ~index - 1;
            }

            return index >= 0 ? this.originalLineNumberToRelativeDiffLineOffsetMap[index].DiffOffset : 0;
        }

        private int TransformDiffLineToDiffLineOffset(int diffLine)
        {
            if (this.lineToClasificationTypeMap.ContainsKey(diffLine) && this.lineToClasificationTypeMap[diffLine] == ClassificationType.ImaginaryLine)
            {
                return -1;
            }

            int index = this.originalLineNumberToRelativeDiffLineOffsetMap.BinarySearch(new DiffLineInfo(diffLine, 0), new DiffLineInfoCurrentComparer());
            if (index < 0)
            {
                index = ~index - 1;
            }

            return index >= 0 ? this.originalLineNumberToRelativeDiffLineOffsetMap[index].DiffOffset : 0;
        }

        public string GetLineNumberString(int lineNumber)
        {
            int offset = TransformDiffLineToDiffLineOffset(lineNumber);
            if (offset == -1)
            {
                return "";
            }

            return (lineNumber - offset + 1).ToString(CultureInfo.CurrentCulture);
        }

        public ClassificationType GetLineDiffClassificationType(int lineNumber)
        {
            if (this.lineToClasificationTypeMap.ContainsKey(lineNumber))
            {
                return this.lineToClasificationTypeMap[lineNumber];
            }

            return ClassificationType.NotModifiedLine;
        }

        private static ClassificationType ConvertDiffBlockTypeToClassificationType(DiffBlockType diffBlockType)
        {
            switch (diffBlockType)
            {
                case DiffBlockType.Deleted:
                    return ClassificationType.DeletedLine;
                case DiffBlockType.Imaginary:
                    return ClassificationType.ImaginaryLine;
                case DiffBlockType.Inserted:
                    return ClassificationType.InsertedLine;
                case DiffBlockType.Modified:
                    return ClassificationType.ModifiedLine;
            }

            throw new ArgumentException("Invalid diff type");
        }

        public int GetLinesCount()
        {
            return this.totalLineCount;
        }

        private class DiffLineInfoOriginalComparer : IComparer<DiffLineInfo>
        {
            public int Compare(DiffLineInfo x, DiffLineInfo y)
            {
                return x.OriginalLineNumber.CompareTo(y.OriginalLineNumber);
            }
        }

        private class DiffLineInfoCurrentComparer : IComparer<DiffLineInfo>
        {
            public int Compare(DiffLineInfo x, DiffLineInfo y)
            {
                return x.CurrentLineNumber.CompareTo(y.CurrentLineNumber);
            }
        }

        private struct DiffLineInfo
        {
            private readonly int originalLine;
            private readonly int diffOffset;

            public int OriginalLineNumber
            {
                get
                {
                    return originalLine;
                }
            }

            public int DiffOffset
            {
                get
                {
                    return diffOffset;
                }
            }

            public int CurrentLineNumber
            {
                get
                {
                    return originalLine + diffOffset;
                }
            }

            public DiffLineInfo(int originalLine, int diffOffset)
            {
                this.originalLine = originalLine;
                this.diffOffset = diffOffset;
            }
        }
    }
}