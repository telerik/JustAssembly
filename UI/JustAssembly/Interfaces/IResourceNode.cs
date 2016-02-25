using JustAssembly.Interfaces;

namespace JustAssembly.Nodes
{
    interface IResourceNode
    {
        IOldToNewTupleMap<string> ResourceMap { get; }

        DifferenceDecoration DifferenceDecoration { get; }
    }
}