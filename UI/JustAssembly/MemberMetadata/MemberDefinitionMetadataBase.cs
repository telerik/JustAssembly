using JustDecompile.External.JustAssembly;
namespace JustAssembly
{
    abstract class MemberDefinitionMetadataBase : MemberMetadataBase
    {
        private bool? isAPIMember;

        public MemberDefinitionMetadataBase(ModuleMetadata parentModule, uint tokenId)
            : base(parentModule.AssemblyPath, tokenId)
        {
            this.Module = parentModule;
        }

        public MemberDefinitionMetadataBase(TypeMetadata typeMetadata, uint tokenId)
            : this(typeMetadata.Module, tokenId)
        {
            this.Type = typeMetadata;
        }

        public abstract MemberType MemberType { get; }

        public override bool IsPublic
        {
            get
            {
                if (this.isAPIMember == null)
                {
                    AccessModifier modifier = GetAccessModifier();
                    this.isAPIMember = modifier == AccessModifier.Protected || modifier == AccessModifier.Public;
                }
                return this.isAPIMember.Value;
            }
        }

        protected abstract AccessModifier GetAccessModifier();

        public ModuleMetadata Module { get; private set; }

        public TypeMetadata Type { get; protected set; }
    }
}