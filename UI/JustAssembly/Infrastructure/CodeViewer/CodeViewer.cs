using JustAssembly.Infrastructure.CodeViewer;
using JustAssembly.ViewModels;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace JustAssembly.Views
{
    public class CodeViewer : TextEditor
    {
        private const int MinFontSize = 8;
        private const int MaxFontSize = 20;

        private DiffLineNumberMargin lineNumberMargin;
        private double viewportWidth;
        // We use this flag, because of bug in the horizontal scrolling.
        // A non maximized window is scrolled all the way to the right. When this window is maximized the TextView scrolls to the new value.
        // When the OnScrollOffsetChanged is entered, the value of the HorizontalScrollMaxumumValue is not updated yet and changes the
        // HorizontalOffsetPercent to invalid value. With this flag we skip this.
        private bool skipNextHorizontalScrollOffsetChange;

        public CodeViewer()
        {
            this.lineNumberMargin = new DiffLineNumberMargin(this.SourceCode);

            this.TextArea.Options.EnableHyperlinks = false;

            this.Loaded += OnLoaded;

            this.PreviewMouseWheel += OnPreviewMouseWheel;
            this.TextView.ScrollOffsetChanged += OnScrollOffsetChanged;
            this.TextView.VisualLinesChanged += OnVisualLinesChanged;
            this.TextView.SizeChanged += OnSizeChanged;

            this.TextArea.Caret.CaretBrush = new SolidColorBrush(Colors.Transparent);
        }

        public TextView TextView
        {
            get
            {
                return this.TextArea.TextView;
            }
        }

        public double HorizontalScrollMaxumumValue
        {
            get
            {
                return this.ExtentWidth - this.viewportWidth;
            }
        }

        public ICodeViewerResults SourceCode
        {
            get { return (ICodeViewerResults)GetValue(SourceCodeProperty); }
            set { SetValue(SourceCodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SourceCode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceCodeProperty =
            DependencyProperty.Register("SourceCode", typeof(ICodeViewerResults), typeof(CodeViewer), new PropertyMetadata(OnSourceCodeChanged));

        public IBackgroundRenderer BackgroundRenderer
        {
            get { return (IBackgroundRenderer)GetValue(BackgroundRendererProperty); }
            set { SetValue(BackgroundRendererProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundRenderer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundRendererProperty =
            DependencyProperty.Register("BackgroundRenderer", typeof(IBackgroundRenderer), typeof(CodeViewer), new PropertyMetadata(OnBackgroundRendererChanged));

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(CodeViewer), new PropertyMetadata(OnVerticalOffsetChanged));

        public double HorizontalOffsetPercent
        {
            get { return (double)GetValue(HorizontalOffsetPercentProperty); }
            set { SetValue(HorizontalOffsetPercentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalOffsetPercent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalOffsetPercentProperty =
            DependencyProperty.Register("HorizontalOffsetPercent", typeof(double), typeof(CodeViewer), new PropertyMetadata(OnHorizontalOffsetPercentChanged));

        public VisibleLines VisibleLines
        {
            get { return (VisibleLines)GetValue(VisibleLinesProperty); }
            set { SetValue(VisibleLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibleLinesProperty =
            DependencyProperty.Register("VisibleLines", typeof(VisibleLines), typeof(CodeViewer));

        public double ScrollingLimit
        {
            get { return (double)GetValue(ScrollingLimitProperty); }
            set { SetValue(ScrollingLimitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollingLimit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollingLimitProperty =
            DependencyProperty.Register("ScrollingLimit", typeof(double), typeof(CodeViewer), new PropertyMetadata(OnScrollingLimitChanged));

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(double), typeof(CodeViewer), new PropertyMetadata(OnFontSizeChanged));

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateVisibleLines();
            this.ScrollingLimit = this.TextView.DocumentHeight;
        }

        void OnScrollOffsetChanged(object sender, EventArgs e)
        {
            this.VerticalOffset = this.TextView.VerticalOffset;
            if (this.skipNextHorizontalScrollOffsetChange)
            {
                this.skipNextHorizontalScrollOffsetChange = false;
                return;
            }

            double newHorizontalOffsetPercent = this.TextView.HorizontalOffset / this.HorizontalScrollMaxumumValue;
            if (!double.IsNaN(newHorizontalOffsetPercent))
            {
                this.HorizontalOffsetPercent = newHorizontalOffsetPercent;
            }
        }

        void OnVisualLinesChanged(object sender, EventArgs e)
        {
            if (this.SourceCode != null)
            {
                UpdateVisibleLines();
            }
            else
            {
                this.VisibleLines = new VisibleLines()
                {
                    FirstLine = 0,
                    LastLine = 0
                };
            }
        }

        void OnPreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            e.Handled = true;
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                e.Handled = true;
                if (this.FontSize + e.Delta / 60 < MinFontSize)
                {
                    this.FontSize = MinFontSize;
                }
                else if (this.FontSize + e.Delta / 60 > MaxFontSize)
                {
                    this.FontSize = MaxFontSize;
                }
                else
                {
                    this.FontSize += (e.Delta / 60);
                }
            }
            else
            {
                if (this.VerticalOffset - e.Delta < 0)
                {
                    this.VerticalOffset = 0;
                }
                else if (this.VerticalOffset - e.Delta > this.ScrollingLimit)
                {
                    this.VerticalOffset = this.ScrollingLimit;
                }
                else
                {
                    this.VerticalOffset -= e.Delta;
                }
            }
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                this.viewportWidth = e.NewSize.Width;
                ScrollToHorizontalOffset(this.HorizontalOffsetPercent * this.HorizontalScrollMaxumumValue);
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (sizeInfo.WidthChanged)
            {
				if (this.VerticalScrollBarVisibility == System.Windows.Controls.ScrollBarVisibility.Hidden)
				{
					this.TextArea.Width = this.ActualWidth;
				}
				else
				{
					this.TextArea.Width = this.ActualWidth - SystemParameters.VerticalScrollBarWidth;
				}
                this.skipNextHorizontalScrollOffsetChange = true;
            }
        }

        protected override LineNumberMargin GetLineNumberMargin()
        {
            return this.lineNumberMargin;
        }

        private void UpdateVisibleLines()
        {
            if (!this.TextView.VisualLinesValid)
            {
                this.TextView.EnsureVisualLines();
            }

            VisualLine firstVisualLine = this.TextView.VisualLines.FirstOrDefault();
            VisualLine lastVisualLine = this.TextView.VisualLines.LastOrDefault();
            if (firstVisualLine != null && lastVisualLine != null)
            {
                this.VisibleLines = new VisibleLines()
                {
                    FirstLine = firstVisualLine.FirstDocumentLine.LineNumber - 1,
                    LastLine = lastVisualLine.LastDocumentLine.LineNumber - 1
                };
            }
        }

        private void ScrollToMember()
        {
            Position position = SourceCode.GetMemberPosition();

            if (position != Position.Empty)
            {
                Selection selection = Selection.Create(this.TextArea, position.Start, position.End);

                this.Select(position.Start, position.Length);

                DispatcherObjectExt.BeginInvoke(() => this.ScrollTo(selection.StartPosition.Line, selection.EndPosition.Column));
            }
        }

        private static void OnSourceCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnSourceCodeChanged();
        }

        private void OnSourceCodeChanged()
        {
            if (this.SourceCode != null)
            {
                this.Document = new TextDocument(this.SourceCode.GetSourceCode());
                if (this.SourceCode.HighlighMember)
                {
                    this.ScrollToMember();
                }

                this.lineNumberMargin.SourceCode = this.SourceCode;
            }
            else
            {
                this.Document = new TextDocument();
            }
        }

        private static void OnBackgroundRendererChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnBackgroundRendererChanged();
        }

        private void OnBackgroundRendererChanged()
        {
            this.TextView.BackgroundRenderers.Add(this.BackgroundRenderer);
        }

        private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnVerticalOffsetChanged((double)e.NewValue);
        }

        private void OnVerticalOffsetChanged(double p)
        {
            if (this.SourceCode != null)
            {
                ScrollToVerticalOffset(p);
            }
        }

        private static void OnHorizontalOffsetPercentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnHorizontalOffsetPercentChanged((double)e.NewValue);
        }

        private void OnHorizontalOffsetPercentChanged(double p)
        {
            p = Math.Max(0, p);
            p = Math.Min(p, 1);

            if (this.SourceCode != null)
            {
                ScrollToHorizontalOffset(p * this.HorizontalScrollMaxumumValue);
            }
        }

        private static void OnScrollingLimitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((double)e.NewValue < (double)e.OldValue)
            {
                ((CodeViewer)d).ScrollingLimit = (double)e.OldValue;
            }
        }

        private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CodeViewer)d).OnFontSizeChanged((double)e.NewValue);
        }

        private void OnFontSizeChanged(double p)
        {
            base.FontSize = p;
            if (this.SourceCode != null)
            {
                this.ScrollingLimit = this.TextView.DocumentHeight;
            }
        }
    }
}
