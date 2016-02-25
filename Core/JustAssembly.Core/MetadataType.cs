using System;

namespace JustAssembly.Core
{
    public enum MetadataType : byte
    {
        Assembly,
        Module,
        Type,
        Field,
        Method,
        Property,
        Event,
        AssemblyReference
    }
}
