using System;
using System.Collections.Generic;
using System.Linq;
using JustDecompile.External.JustAssembly;

namespace JustAssembly
{
    static class GlobalDecompilationResultsRepository
    {
        public static readonly IDecompilationResultsRepository Instance = new DecompilationResultsRepository();
    }

    class DecompilationResultsRepository : IDecompilationResultsRepository
    {
        private readonly IDictionary<string, IAssemblyDecompilationResults> cache;

        public DecompilationResultsRepository()
        {
            this.cache = new Dictionary<string, IAssemblyDecompilationResults>();
        }

        public void AddDecompilationResult(string assemblyPath, IAssemblyDecompilationResults result)
        {
            cache[assemblyPath] = result;
        }

        public bool RemoveByAssemblyPath(string assemblyPaht)
        {
            return cache.Remove(assemblyPaht);
        }

        public bool TryGetDecompilationResult(string assemblyPath, uint moduleId, uint typeId, out IDecompilationResults target)
        {
            target = null;

            IAssemblyDecompilationResults decompilationResult;

            if (cache.TryGetValue(assemblyPath, out decompilationResult))
            {
                IModuleDecompilationResults moduleResult = decompilationResult.ModuleDecompilationResults.FirstOrDefault(m => m.ModuleToken == moduleId);

                if (moduleResult != null)
                {
                    bool succeeded = moduleResult.TypeDecompilationResults.TryGetValue(typeId, out target);

                    return succeeded;
                }
            }
            return false;
        }

        public bool ContainsAssembly(string assemblyPath)
        {
            return cache.ContainsKey(assemblyPath);
		}
		
        public bool TryGetAssemblyDecompilationResult(string assemblyPath, out IAssemblyDecompilationResults decompilationResult)
        {
            decompilationResult = null;

            if (string.IsNullOrWhiteSpace(assemblyPath))
            {
                return false;
            }
            return cache.TryGetValue(assemblyPath, out decompilationResult);
        }
    }
}
