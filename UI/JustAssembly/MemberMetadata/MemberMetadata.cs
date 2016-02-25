using System;
using JustDecompile.External.JustAssembly;

namespace JustAssembly
{
    class MemberMetadata : MemberDefinitionMetadataBase
    {
        private readonly MemberType memberType;

        public MemberMetadata(TypeMetadata type, MemberType memberType, uint memberId)
            : base(type, memberId)
        {
            this.memberType = memberType;
        }

        public override MemberType MemberType
        {
            get { return this.memberType; }
        }

        protected override string GetNameInternal()
        {
            return Decompiler.GetMemberName(AssemblyPath, Type.Module.TokenId, Type.TokenId, TokenId, SupportedLanguage.CSharp);
        }

        public override string ToString()
        {
            return this.GetSignature();
        }

        public string GetSignature()
        {
            string fullName = this.GetName();
            int index = fullName.IndexOf(" : ");
            return index > 0 ? fullName.Substring(0, index) : fullName;
        }

        protected override AccessModifier GetAccessModifier()
        {
            return Decompiler.GetMemberAccessModifier(AssemblyPath, Type.Module.TokenId, Type.TokenId, TokenId, SupportedLanguage.CSharp);
        }
    }
}