using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using ICSharpCode.TreeView;
using Mono.Cecil;
using JustDecompile.EngineInfrastructure;
using JustDecompile.Tools.MSBuildProjectBuilder;
using JustDecompile.External.JustAssembly;

using JustAssembly.Interfaces;
using JustAssembly.MergeUtilities;
using JustAssembly.Nodes.APIDiff;
using JustAssembly.Dialogs.DangerousResource;

namespace JustAssembly.Nodes
{
    class AssemblyNode : DecompiledMemberNodeBase
    {
        public readonly IOldToNewTupleMap<string> TypesMap;
        private readonly IProgressNotifier progressNotifier;

        private IDecompilationResults oldDecompilationResults;
        private IDecompilationResults newDecompilationResults;

        private bool areChildrenLoaded;

        public AssemblyNode(IOldToNewTupleMap<string> typesMap, ItemNodeBase parent, APIDiffInfo apiDiffInfo, IProgressNotifier progressNotifier, FilterSettings filterSettings)
            : base(Path.GetFileName(typesMap.GetFirstNotNullItem()), parent, apiDiffInfo, filterSettings)
        {
            this.TypesMap = typesMap;

            this.progressNotifier = progressNotifier;

            this.areChildrenLoaded = false;
        }

        public override object Icon
        {
            get { return ImagesResourceStrings.AssemblyNode; }
        }

        public override NodeType NodeType
        {
            get { return NodeType.AssemblyNode; }
        }

        public uint TotalItems { get; set; }

        public override string FullName
        {
            get
            {
                return TypesMap.GetFirstNotNullItem();
            }
        }

        public override MemberDefinitionMetadataBase OldMemberMetada
        {
            get { return null; }
        }

        public override MemberDefinitionMetadataBase NewMemberMetada
        {
            get { return null; }
        }

        public override IDecompilationResults OldDecompileResult
        {
            get
            {
                GetAttributeDecompilationResult(this.TypesMap.OldType, ref this.oldDecompilationResults);
                return this.oldDecompilationResults;
            }
            set
            {
                this.oldDecompilationResults = value;
            }
        }

        public override IDecompilationResults NewDecompileResult
        {
            get
            {
                GetAttributeDecompilationResult(this.TypesMap.NewType, ref this.newDecompilationResults);
                return this.newDecompilationResults;
            }
            set
            {
                this.newDecompilationResults = value;
            }
        }

        private void GetAttributeDecompilationResult(string filePath, ref IDecompilationResults decompilationResults)
        {
            if (decompilationResults == null)
            {
                IAssemblyDecompilationResults assemblyResult = GetAssemblyResult(filePath);
                if (assemblyResult == null)
                {
                    return;
                }
                decompilationResults = assemblyResult.AssemblyAttributesDecompilationResults;
            }
        }

        private IAssemblyDecompilationResults GetAssemblyResult(string path)
        {
            IAssemblyDecompilationResults assemblyResults;

            return GlobalDecompilationResultsRepository.Instance.TryGetAssemblyDecompilationResult(path, out assemblyResults) ? assemblyResults : null;
        }

        protected override void LoadChildren()
        {
            if (!areChildrenLoaded)
            {
                this.LoadItemsAsync();
            }
        }

        public Task LoadItemsAsync(Action cancelAction = null)
        {
            CancellationToken cancellationToken = progressNotifier.GetCanellationToken();

            cancellationToken.Register(() =>
            {
                if (cancelAction != null)
                {
                    cancelAction();
                }
            });
            this.progressNotifier.IsBusy = true;
            this.progressNotifier.IsIndeterminate = false;
            this.progressNotifier.LoadingMessage = "Loading...";

            var task = Task.Factory.StartNew(() =>
            {
				try
				{
					int assemblyCount = TypesMap.OldType != null && TypesMap.NewType != null ? 2 : 1;
					int assemblyNumber = 1;

					if (TypesMap.OldType != null && !GlobalDecompilationResultsRepository.Instance.ContainsAssembly(TypesMap.OldType))
					{
						progressNotifier.LoadingMessage = string.Format("Loading assembly {0} of {1}", assemblyNumber, assemblyCount);

                        AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(TypesMap.NewType, new ReaderParameters(GlobalAssemblyResolver.Instance));
                        bool shouldDecompileDangerousResources = this.ShouldDecompileDangerousResources(assembly, TypesMap.OldType, AssemblyType.Old);

                        IAssemblyDecompilationResults r1 = Decompiler.GenerateFiles(TypesMap.OldType,
																						  assembly,
																						  GenerationProjectInfoMap.OldType.OutputPath,
																						  SupportedLanguage.CSharp,
																						  cancellationToken,
                                                                                          shouldDecompileDangerousResources,
																						  progressNotifier);

						cancellationToken.ThrowIfCancellationRequested();

						GlobalDecompilationResultsRepository.Instance.AddDecompilationResult(TypesMap.OldType, r1);

                        TrackFrameworkVersionAndAssemblyType(TypesMap.OldType);

						assemblyNumber++;
					}
					if (TypesMap.NewType != null && !GlobalDecompilationResultsRepository.Instance.ContainsAssembly(TypesMap.NewType))
					{
						progressNotifier.LoadingMessage = string.Format("Loading assembly {0} of {1}", assemblyNumber, assemblyCount);
						progressNotifier.Progress = 0;

                        AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(TypesMap.NewType, new ReaderParameters(GlobalAssemblyResolver.Instance));
                        bool shouldDecompileDangerousResources = this.ShouldDecompileDangerousResources(assembly, TypesMap.NewType, AssemblyType.New);

                        IAssemblyDecompilationResults r2 = Decompiler.GenerateFiles(TypesMap.NewType,
                                                                                          assembly,
																						  GenerationProjectInfoMap.NewType.OutputPath,
																						  SupportedLanguage.CSharp,
																						  cancellationToken,
                                                                                          shouldDecompileDangerousResources,
																						  progressNotifier);

						cancellationToken.ThrowIfCancellationRequested();

                        GlobalDecompilationResultsRepository.Instance.AddDecompilationResult(TypesMap.NewType, r2);

                        TrackFrameworkVersionAndAssemblyType(TypesMap.NewType);
					}
					List<SharpTreeNode> moduleNodes = this.GetMergedModules(true).ToList();

					this.differenceDecoration = this.GetDifferenceDecoration(moduleNodes);

                    var defaultResourceNode = new DefaultResourceNode(TypesMap, this, this.GenerationProjectInfoMap, this.FilterSettings);

					DispatcherObjectExt.BeginInvoke(() =>
					{
						cancellationToken.ThrowIfCancellationRequested();

                        foreach (ModuleNode node in moduleNodes)
                        {
                            this.Children.Add(node);
                        }

						this.Children.Add(defaultResourceNode);

						this.OnChildrenLoaded();

						this.progressNotifier.Completed();
					}, DispatcherPriority.Background);
				}
				catch (Exception ex)
				{
					var exceptionStringBuilder = new StringBuilder();

					exceptionStringBuilder.Append(ex.Message)
						.AppendLine()
						.AppendLine(ex.StackTrace);

					DispatcherObjectExt.BeginInvoke(
						() =>
						{
							ToolWindow.ShowDialog(new ErrorMessageWindow(exceptionStringBuilder.ToString(), "Exception"), width: 800, height: 500);
							this.progressNotifier.Completed();
						},
						DispatcherPriority.Background);

					throw ex;
				}

            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);

            return task;
        }

        public void SetDifferenceDecoration()
        {
            List<SharpTreeNode> modules = this.GetMergedModules(false);

            this.differenceDecoration = this.GetDifferenceDecoration(modules);
        }

        public bool HasInvalidAssemblies(AssemblyHelper assemblyHelper)
        {
            bool isOldInvalid = false;
            bool isNewInvalid = false;

            if (!assemblyHelper.IsValidClrAssembly(TypesMap.OldType))
            {
                isOldInvalid = true;

                assemblyHelper.AddNotSupportedFiles(TypesMap.OldType);
            }
            if (!assemblyHelper.IsValidClrAssembly(TypesMap.NewType))
            {
                isNewInvalid = true;

                assemblyHelper.AddNotSupportedFiles(TypesMap.NewType);
            }
            return isOldInvalid || isNewInvalid;
        }

        private bool ShouldDecompileDangerousResources(AssemblyDefinition assembly, string assemblyPath, AssemblyType assemblyType)
        {
            try
            {
                Dictionary<ModuleDefinition, Mono.Collections.Generic.Collection<Resource>> assemblyResources = Utilities.GetResources(assembly);

                bool containsDangerousResources = assemblyResources.SelectMany(rc => rc.Value)
                                                                   .Any(resource => DangerousResourceIdentifier.IsDangerousResource(resource));

                bool decompileDangerousResources = false;
                if (containsDangerousResources)
                {
                    DispatcherObjectExt.Invoke(() =>
                    {
                        DangerousResourceDialog dialog = new DangerousResourceDialogWithAnalyticsTracking(Path.GetFileName(assemblyPath), assemblyType);
                        if (dialog.Show() == DangerousResourceDialogResult.Yes)
                        {
                            decompileDangerousResources = true;
                        }
                    });
                }

                return decompileDangerousResources;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private List<SharpTreeNode> GetMergedModules(bool shouldBeExpanded)
        {
            var moduleMergeManager = new ModuleManager(TypesMap);

            LoadAPIItemsContext context = this.apiDiffInfo.GenerateLoadAPIItemsContext();

            List<SharpTreeNode> moduleNodes = moduleMergeManager.GetMergedCollection()
                                                             .Where(ApiOnlyFilter)
                                                             .Select(tuple => (SharpTreeNode)GenerateModuleNode(tuple, context, shouldBeExpanded))
                                                             .ToList();

            context.Validate();

            return moduleNodes;
        }

        private ModuleNode GenerateModuleNode(IOldToNewTupleMap<ModuleMetadata> metadataTuple, LoadAPIItemsContext context, bool shouldBeExpanded)
        {
            return new ModuleNode(metadataTuple, this, context.GenerateAPIDiffInfo(metadataTuple), shouldBeExpanded, this.FilterSettings);
        }

        protected override DifferenceDecoration GetDifferenceDecoration()
        {
            return this.GetDifferenceDecoration(this.Children);
        }

        private DifferenceDecoration GetDifferenceDecoration(IEnumerable<SharpTreeNode> modules)
        {
            if (this.CanUseParentDiffDecoration)
            {
                return ParentNode.DifferenceDecoration;
            }

            if (this.apiDiffInfo != null)
            {
                if (this.TypesMap.NewType == null)
                {
                    return DifferenceDecoration.Deleted;
                }
                if (this.TypesMap.OldType == null)
                {
                    return this.BreakingChangesOnly ? DifferenceDecoration.NoDifferences : DifferenceDecoration.Added;
                }
                return this.apiDiffInfo.APIDiffItem.GetDifferenceDecoration(this.BreakingChangesOnly);
            }

            if (string.IsNullOrWhiteSpace(TypesMap.OldType))
            {
                return DifferenceDecoration.Added;
            }
            else if (string.IsNullOrWhiteSpace(TypesMap.NewType))
            {
                return DifferenceDecoration.Deleted;
            }
            foreach (ModuleNode moduleNode in modules)
            {
                if (moduleNode.DifferenceDecoration == DifferenceDecoration.Modified)
                {
                    return DifferenceDecoration.Modified;
                }
            }
            return DifferenceDecoration.NoDifferences;
        }

        protected override void OnChildrenLoaded()
        {
            base.OnChildrenLoaded();

            this.areChildrenLoaded = true;
        }

        private void TrackFrameworkVersionAndAssemblyType(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            try
            {
                AssemblyDefinition assemblyDefinition = GlobalAssemblyResolver.Instance.GetAssemblyDefinition(filePath);
                if (assemblyDefinition != null)
                {
                    AssemblyInfo assemblyInfo = NoCacheAssemblyInfoService.Instance.GetAssemblyInfo(assemblyDefinition, EmptyResolver.Instance);
                    Configuration.Analytics.TrackFeature("LoadedAssemblyFrameworkVersion." + assemblyInfo.ModulesFrameworkVersions[assemblyDefinition.MainModule].ToString());
                    Configuration.Analytics.TrackFeature("LoadedAssemblyType." + assemblyInfo.AssemblyTypes.ToString().Replace(", ", "_"));
                }
            }
            catch (Exception) { }
        }
    }
}