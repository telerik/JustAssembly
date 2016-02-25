using JustDecompile.External.JustAssembly;

namespace JustAssembly
{
    interface IDecompilationResultsRepository
    {
        bool ContainsAssembly(string assemblyPath);

        bool RemoveByAssemblyPath(string assemblyPaht);

        void AddDecompilationResult(string assemblyPath, IAssemblyDecompilationResults result);

        bool TryGetAssemblyDecompilationResult(string filePath, out IAssemblyDecompilationResults target);

        bool TryGetDecompilationResult(string filePath, uint moduleId, uint typeId, out IDecompilationResults target);
    }
}