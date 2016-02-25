using JustDecompile.External.JustAssembly;

namespace JustAssembly
{
    public class ModuleMetadata : MemberMetadataBase
    {
        private bool? containsPublicTypes;

        public ModuleMetadata(string assemblyPath, uint tokenId)
            : base(assemblyPath, tokenId)
        {
        }

        protected override string GetNameInternal()
        {
            return Decompiler.GetModuleName(AssemblyPath, TokenId);
        }

        public override bool IsPublic
        {
            get
            {
                if (containsPublicTypes == null)
                {
                    containsPublicTypes = false;
                    foreach (uint typeId in Decompiler.GetModuleTypes(this.AssemblyPath, this.TokenId))
                    {
                        AccessModifier access = Decompiler.GetTypeAccessModifier(this.AssemblyPath, this.TokenId, typeId);
                        if (access == AccessModifier.Public || access == AccessModifier.Protected)
                        {
                            containsPublicTypes = true;
                            break;
                        }
                    }
                }
                return this.containsPublicTypes.Value;
            }
        }
    }
}