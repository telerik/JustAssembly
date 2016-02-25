using System;
using System.Collections.Generic;
using System.Linq;
using JustAssembly.Extensions;
using JustAssembly.Interfaces;
using JustDecompile.External.JustAssembly;

namespace JustAssembly.MergeUtilities
{
    class ModuleManager : MergeManagerBase<string, ModuleMetadata>
    {
        public ModuleManager(IOldToNewTupleMap<string> oldToNewTupleMap)
            : base(oldToNewTupleMap)
        {
        }

        public override IEnumerable<IOldToNewTupleMap<ModuleMetadata>> GetMergedCollection()
        {
            List<ModuleMetadata> leftModules = null;
            List<ModuleMetadata> rigthModules = null;

            if (TryGetSortedByNameModules(tupleMap.OldType, out leftModules) | TryGetSortedByNameModules(tupleMap.NewType, out rigthModules))
            {
                return leftModules.Merge(rigthModules, ModuleNameComparer);
            }
            else
            {
                return Enumerable.Empty<IOldToNewTupleMap<ModuleMetadata>>();
            }
        }

        public static bool AreAssembliesEquals(string oldAssemblyPath, string newAssemblyPath)
        {
            List<ModuleMetadata> oldModules = GetSortedByNameModules(oldAssemblyPath);
            List<ModuleMetadata> newModules = GetSortedByNameModules(newAssemblyPath);

            if (oldModules.Count != newModules.Count)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < oldModules.Count; i++)
                {
                    ModuleMetadata oldModuleDef = oldModules[i];
                    ModuleMetadata newModuleDef = newModules[i];

                    if (!AreModulesEquals(oldModuleDef, newModuleDef))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public static bool AreModulesEquals(ModuleMetadata oldModuleDef, ModuleMetadata newModuleDef)
        {
            string oldModuleName = oldModuleDef.GetName();
            string newModuleName = newModuleDef.GetName();

            if (oldModuleName != newModuleName)
            {
                return false;
            }
            else
            {
                byte[] oldBinaries = Decompiler.GetImageData(oldModuleDef.AssemblyPath, oldModuleDef.TokenId);
                byte[] newBinaries = Decompiler.GetImageData(newModuleDef.AssemblyPath, newModuleDef.TokenId);

                if (oldBinaries.Length != newBinaries.Length)
                {
                    return false;
                }
                else
                {
                    for (int j = 0; j < oldBinaries.Length; j++)
                    {
                        if (oldBinaries[j] != newBinaries[j])
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private static List<ModuleMetadata> GetSortedByNameModules(string assemblyPath)
        {
            List<ModuleMetadata> modulesMetadatas = Decompiler.GetAssemblyModules(assemblyPath).Select(m => new ModuleMetadata(assemblyPath, m)).ToList();

            modulesMetadatas.Sort(ModuleNameComparer);

            return modulesMetadatas;
        }

        private static bool TryGetSortedByNameModules(string assemblyPath, out List<ModuleMetadata> modulesMetadatas)
        {
            if (string.IsNullOrWhiteSpace(assemblyPath))
            {
                modulesMetadatas = new List<ModuleMetadata>();

                return false;
            }
            modulesMetadatas = GetSortedByNameModules(assemblyPath);

            return true;
        }

        private static int ModuleNameComparer(ModuleMetadata oldModuleToken, ModuleMetadata newModuleToken)
        {
            string oldModuleName = oldModuleToken.GetName();

            string newModuleName = newModuleToken.GetName();

            return string.Compare(oldModuleName, newModuleName, true);
        }
    }
}
