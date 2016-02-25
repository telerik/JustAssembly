using System;
using System.Collections.Generic;
using System.Linq;

namespace JustAssembly.Interfaces
{
    public interface IOldToNewTupleMap<out T>
        where T : class
    {
        T OldType { get; }

        T NewType { get; }

        T GetFirstNotNullItem();
    }
}
