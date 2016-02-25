using System;
using System.Linq;

namespace JustAssembly
{
    public abstract class MemberMetadataBase
    {
        private string name;

        public MemberMetadataBase(string assemblyPath, uint tokenId)
        {
            this.AssemblyPath = assemblyPath;

            this.TokenId = tokenId;
        }

        public readonly uint TokenId;

        public readonly string AssemblyPath;

        public string GetName()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                this.name = this.GetNameInternal();
            }
            return this.name;
        }

        protected abstract string GetNameInternal();

        public abstract bool IsPublic { get; }
    }
}
