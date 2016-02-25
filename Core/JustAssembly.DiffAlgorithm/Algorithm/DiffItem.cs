using System;

namespace JustAssembly.DiffAlgorithm.Algorithm
{
    public struct DiffItem
    {
        /// <summary>Number of changes in Data A.</summary>
        public int DeletedA { get; set; }
        /// <summary>Number of changes in Data A.</summary>
        public int InsertedB { get; set; }
        /// <summary>Start Line number in Data A.</summary>
        public int StartA { get; set; }
        /// <summary>Start Line number in Data B.</summary>
        public int StartB { get; set; }
        public override string ToString()
        {
            return string.Format("StartA: {0} DeletedA: {1} StartB: {2} InsertedB: {3}", StartA, DeletedA, StartB, InsertedB);
        }
    }
}
