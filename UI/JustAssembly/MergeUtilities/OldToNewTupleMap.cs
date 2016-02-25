using System;
using JustAssembly.Interfaces;

namespace JustAssembly.MergeUtilities
{
    class OldToNewTupleMap<T> : IOldToNewTupleMap<T> 
        where T : class
    {
        public OldToNewTupleMap(T oldType, T newType)
        {
            this.OldType = oldType;

            this.NewType = newType;
        }

        public T OldType { get; private set; }

        public T NewType { get; private set; }

        public T GetFirstNotNullItem()
        {
            return OldType ?? NewType;            
        }
    }
}