using System.Collections.Generic;
using System.IO;
using System.Text;
using JustAssembly.Interfaces;
using JustAssembly.Nodes.APIDiff;
using JustDecompile.External.JustAssembly;

namespace JustAssembly.Nodes
{
    abstract class DecompiledMemberNodeBase : ItemNodeBase
    {
        public DecompiledMemberNodeBase(string name, ItemNodeBase parent, APIDiffInfo apiDiffInfo, FilterSettings filterSettings)
            : base(name, parent, apiDiffInfo, filterSettings)
        {
            DecompiledMemberNodeBase parentMember = parent as DecompiledMemberNodeBase;

            if (parentMember != null)
            {
                this.OldDecompileResult = parentMember.OldDecompileResult;
                this.NewDecompileResult = parentMember.NewDecompileResult;
            }
        }

        public abstract MemberDefinitionMetadataBase OldMemberMetada { get; }

        public abstract MemberDefinitionMetadataBase NewMemberMetada { get; }

        public virtual IDecompilationResults OldDecompileResult { get; set; }

        public virtual IDecompilationResults NewDecompileResult { get; set; }

        public virtual string OldSource { get { return GetFullSource(this.OldDecompileResult); } }

        public virtual string NewSource { get { return GetFullSource(this.NewDecompileResult); } }

        protected string GetFullSource(IDecompilationResults result)
        {
            if (result == null)
            {
                return string.Empty;
            }

            try
            {
                return File.ReadAllText(result.FilePath);
            }
            catch
            {
                return string.Empty;
            }
        }

        protected string GetMemberSource(IDecompilationResults result, MemberDefinitionMetadataBase metadata)
        {
            if (metadata == null || result == null)
            {
                return string.Empty;
            }

            IOffsetSpan offsetSpan;
            if (result.MemberTokenToDecompiledCodeMap.TryGetValue(metadata.TokenId, out offsetSpan))
            {
                try
                {
                    string source = File.ReadAllText(result.FilePath);
                    return GetMemberSource(source, offsetSpan, metadata.TokenId, result);
                }
                catch
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        private string GetMemberSource(string sourceCode, IOffsetSpan memberOffset, uint memberId, IDecompilationResults decompilationResult)
        {
            StringBuilder memberSourceBuilder = new StringBuilder();

            memberSourceBuilder.Append(GetMemberSourceFromMap(sourceCode, decompilationResult.MemberTokenToAttributesMap, memberId))
                               .Append(GetMemberSourceFromMap(sourceCode, decompilationResult.MemberTokenToDocumentationMap, memberId))
                               .Append(sourceCode.Substring(memberOffset.StartOffset, memberOffset.EndOffset - memberOffset.StartOffset + 1));

            return memberSourceBuilder.ToString();
        }

        private string GetMemberSourceFromMap(string sourceCode, IDictionary<uint, IOffsetSpan> memberIdToOffsetMap, uint memberId)
        {
            IOffsetSpan offset;

            if (memberIdToOffsetMap.TryGetValue(memberId, out offset))
            {
                return sourceCode.Substring(offset.StartOffset, offset.EndOffset - offset.StartOffset + 1);
            }
            return string.Empty;
        }
    }
}