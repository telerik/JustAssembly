using System;
using System.Windows;
using JustAssembly.Infrastructure.CodeViewer;
using JustAssembly.Interfaces;
using JustAssembly.DiffAlgorithm;
using JustAssembly.DiffAlgorithm.Models;
using JustAssembly.Nodes;

namespace JustAssembly.ViewModels
{
    internal abstract class CodeDiffTabItemBase<T> : TabSourceItemBase
        where T : DecompiledMemberNodeBase
    {
        private const int InitialFontSize = 12;

        private double verticalOffset;
        private double horizontalOffsetPercent;
        private double fontSize;

        protected string toolTip;

        protected readonly T instance;

        private VisibleLines visibleLines;

        private double scrollingLimit;

        public CodeDiffTabItemBase(T param)
        {
            this.instance = param;

            this.FontSize = InitialFontSize;
        }

        public double VerticalOffset
        {
            get
            {
                return verticalOffset;
            }
            set
            {
                if (verticalOffset != value)
                {
                    verticalOffset = value;
                    this.RaisePropertyChanged("VerticalOffset");
                }
            }
        }

        public VisibleLines VisibleLines
        {
            get
            {
                return this.visibleLines;
            }
            set
            {
                if (this.visibleLines != value)
                {
                    this.visibleLines = value;
                    this.RaisePropertyChanged("VisibleLines");
                }
            }
        }

        public double ScrollingLimit
        {
            get
            {
                return this.scrollingLimit;
            }
            set
            {
                if (this.scrollingLimit != value)
                {
                    this.scrollingLimit = value;
                    this.RaisePropertyChanged("ScrollingLimit");
                }
            }
        }

        public double HorizontalOffsetPercent
        {
            get
            {
                return this.horizontalOffsetPercent;
            }
            set
            {
                if (this.horizontalOffsetPercent != value)
                {
                    this.horizontalOffsetPercent = value;
                    this.RaisePropertyChanged("HorizontalOffsetPercent");
                }
            }
        }

        public double FontSize
        {
            get
            {
                return this.fontSize;
            }
            set
            {
                if (this.fontSize != value)
                {
                    this.fontSize = value;
                    this.RaisePropertyChanged("FontSize");
                }
            }
        }

        public override TabKind TabKind
        {
            get
            {
                return TabKind.JustAssembly;
            }
        }

        public ICodeViewerResults LeftSourceCode { get; set; }

        public ICodeViewerResults RightSourceCode { get; set; }

        protected virtual void ApplyDiff()
        {
            DiffResult diffResult = DiffHelper.Diff(instance.OldSource, instance.NewSource);

            if (this.LeftSourceCode != null)
            {
                this.LeftSourceCode.ApplyDiffInfo(diffResult.File);
            }
            if (this.RightSourceCode != null)
            {
                this.RightSourceCode.ApplyDiffInfo(diffResult.ModifiedFile);
            }
            this.RaisePropertyChanged("LeftSourceCode");
            this.RaisePropertyChanged("RightSourceCode");
        }

        public override void OnProjectFileGenerated(JustDecompile.External.JustAssembly.IFileGeneratedInfo args)
        {

        }

        public override string ToolTip
        {
            get
            {
                return toolTip;
            }
        }
    }

    public class VisibleLines
    {
        public int FirstLine { get; set; }
        public int LastLine { get; set; }
    }
}