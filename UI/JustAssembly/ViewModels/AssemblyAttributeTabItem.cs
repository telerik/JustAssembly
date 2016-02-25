using System;
using JustAssembly.Infrastructure.CodeViewer;
using JustDecompile.External.JustAssembly;
using JustAssembly.Nodes;

namespace JustAssembly.ViewModels
{
    class AssemblyAttributeTabItem : CodeDiffTabItemBase<AssemblyNode>
    {
        public AssemblyAttributeTabItem(AssemblyNode assemblyNode)
            : base(assemblyNode) { }

        public override void LoadContent()
        {
            this.IsIndeterminate = true;
            this.IsBusy = true;
            IAssemblyDecompilationResults oldAssemblyResult;
            IAssemblyDecompilationResults newAssemblyResult;

            this.header = GetTabTitle(instance);
            this.toolTip = GetTabToolTip(instance);

            if (instance.TypesMap.OldType != null &&
                GlobalDecompilationResultsRepository.Instance.TryGetAssemblyDecompilationResult(instance.TypesMap.OldType, out oldAssemblyResult))
            {
                this.LeftSourceCode = new DecompiledSourceCode(instance.OldMemberMetada, oldAssemblyResult.AssemblyAttributesDecompilationResults, instance.OldSource);
            }
            if (instance.TypesMap.NewType != null &&
                GlobalDecompilationResultsRepository.Instance.TryGetAssemblyDecompilationResult(instance.TypesMap.NewType, out newAssemblyResult))
            {
                this.RightSourceCode = new DecompiledSourceCode(instance.NewMemberMetada, newAssemblyResult.AssemblyAttributesDecompilationResults, instance.NewSource);
            }
            this.ApplyDiff();
            this.IsBusy = false;
        }
  
        private string GetTabTitle(AssemblyNode instance)
        {
            return string.Format("{0} Attributes", instance.Name);
        }

        private string GetTabToolTip(AssemblyNode instance) 
        {
            return string.Format("{0} Attributes", instance.Name);
        }
    }
}