using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using JustAssembly.Infrastructure;
using JustAssembly.Infrastructure.CodeViewer;
using JustAssembly.ViewModels;
using System.Windows.Input;

namespace JustAssembly.Views
{
    /// <summary>
    /// Interaction logic for SideSummary.xaml
    /// </summary>
    partial class SideSummary : UserControl
    {
        double lineWidth = 2;

        public SideSummary()
        {
            InitializeComponent();
        }

        #region Dependency properties

        public VisibleLines VisibleLines
        {
            get { return (VisibleLines)GetValue(VisibleLinesProperty); }
            set { SetValue(VisibleLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibleLinesProperty =
            DependencyProperty.Register("VisibleLines", typeof(VisibleLines), typeof(SideSummary), new PropertyMetadata(OnVisibleLinesChanged));

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(SideSummary), new PropertyMetadata(OnVerticalOffsetChanged));

        public ICodeViewerResults LeftSourceCode
        {
            get { return (ICodeViewerResults)GetValue(LeftSourceCodeProperty); }
            set { SetValue(LeftSourceCodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftSourceCode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftSourceCodeProperty =
            DependencyProperty.Register("LeftSourceCode", typeof(ICodeViewerResults), typeof(SideSummary));

        public ICodeViewerResults RightSourceCode
        {
            get { return (ICodeViewerResults)GetValue(RightSourceCodeProperty); }
            set { SetValue(RightSourceCodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightSourceCode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightSourceCodeProperty =
            DependencyProperty.Register("RightSourceCode", typeof(ICodeViewerResults), typeof(SideSummary));

        public double ScrollingLimit
        {
            get { return (double)GetValue(ScrollingLimitProperty); }
            set { SetValue(ScrollingLimitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollingLimit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollingLimitProperty =
            DependencyProperty.Register("ScrollingLimit", typeof(double), typeof(SideSummary));

        
        #endregion

        private static void OnVisibleLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SideSummary)d).OnVisibleLinesChanged();
        }

        private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SideSummary)d).OnVerticalOffsetChanged();
        }

        private void OnVerticalOffsetChanged()
        {
            DrawViewRectangle();
        }

        private void OnVisibleLinesChanged()
        {
            DrawViewRectangle();
        }

        public void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            double newTop = Canvas.GetTop(viewWindow) + e.VerticalChange;
            if (newTop < 0)
            {
                Canvas.SetTop(viewWindow, 0);
            }
            else if (newTop > canvas.RenderSize.Height - viewWindow.Height)
            {
                Canvas.SetBottom(viewWindow, canvas.RenderSize.Height);
            }
            else
            {
                Canvas.SetTop(viewWindow, Canvas.GetTop(viewWindow) + e.VerticalChange);
            }
            this.VerticalOffset += ((e.VerticalChange * ScrollingLimit) / canvas.RenderSize.Height);
            if (VerticalOffset < 0)
            {
                VerticalOffset = 0;
            }
            if (VerticalOffset > ScrollingLimit)
            {
                VerticalOffset = ScrollingLimit;
            }
        }

        public int RowCount 
        {
            get 
            {
                int leftLines = LeftSourceCode == null? 0 : LeftSourceCode.GetLinesCount();
                int rightLines = RightSourceCode == null ? 0 : RightSourceCode.GetLinesCount();
                return leftLines > rightLines ? leftLines : rightLines; 
            }
        }

        private void DrawLines() 
        {
            canvas.Children.Clear();
            DrawSourceLines();
        }
  
        private void DrawViewRectangle()
        {
            double top = GetLineHorisontalCoordinates(VisibleLines.FirstLine);
            double bottom = GetLineHorisontalCoordinates(VisibleLines.LastLine);
            top -= 1;
            bottom += 1;
            viewWindow.Width = canvas.RenderSize.Width + 2;
            viewWindow.Height = bottom - top;

            if (viewWindow.Height <= viewWindow.MinHeight)
            {
                PlaceMinHeightWindow();
            }
            else
            {
                Canvas.SetTop(viewWindow, top);
            }
        }
  
        private void PlaceMinHeightWindow()
        {
            double topLineOffset = GetLineHorisontalCoordinates(VisibleLines.FirstLine);
            double bottomLineOffset = GetLineHorisontalCoordinates(VisibleLines.LastLine);

            double desiredTop = (topLineOffset +bottomLineOffset - viewWindow.MinHeight) / 2;
            if (desiredTop < 0)
            {
                // the window will go out of the canvas it gets centered.
                Canvas.SetTop(viewWindow, 0);
                return;
            }
            else if(desiredTop + viewWindow.MinHeight >= canvas.RenderSize.Height)
            {
                // the window will go out of the canvas it gets centered.
                Canvas.SetTop(viewWindow, canvas.RenderSize.Height - viewWindow.MinHeight);
                return;
            }

            Canvas.SetTop(viewWindow, desiredTop);
        }
  
        private void DrawSourceLines()
        {
            lineWidth = Math.Ceiling(canvas.RenderSize.Height / RowCount);
            if (lineWidth < 1)
            {
                lineWidth = 1;
            }
            if (LeftSourceCode == null && RightSourceCode == null)
            {
                return;    
            }
            for (int i = 0; i < RowCount; i++)
            {
                ClassificationType lineType = GetLineDiffClassificationType(i);
                if (lineType == ClassificationType.NotModifiedLine || lineType == ClassificationType.ImaginaryLine)
                {
                    // There is no difference in this line or the line is imaginary, so nothing needs to be drawn.
                    continue;
                }

                var myLine = new Line();
                double lineYCoordinates = GetLineHorisontalCoordinates(i);
                myLine.Stroke = new SolidColorBrush(DiffBackgroundRenderer.GetColorFromClassificationType(lineType));
                myLine.X1 = 0;
                myLine.X2 = canvas.RenderSize.Width;
                myLine.Y1 = lineYCoordinates;
                myLine.Y2 = lineYCoordinates;
                myLine.UseLayoutRounding = true;

                myLine.StrokeThickness = lineWidth;
                canvas.Children.Add(myLine);
            }
        }

        private ClassificationType GetLineDiffClassificationType(int i)
        {
            ClassificationType result = ClassificationType.NotModifiedLine;
            if (LeftSourceCode != null)
            {
                result = LeftSourceCode.GetLineDiffClassificationType(i);
                if (result == ClassificationType.ImaginaryLine)
                {
                    return RightSourceCode.GetLineDiffClassificationType(i);
                }
                else 
                {
                    return result;
                }
            }
            else if (RightSourceCode != null)
            {
                result = RightSourceCode.GetLineDiffClassificationType(i);
            }
            return result;
        }
  
        private double GetLineHorisontalCoordinates(int i)
        {
            if (RowCount * lineWidth + lineWidth / 2 < canvas.RenderSize.Height)
            {
                return i * lineWidth + lineWidth / 2;
            }
            return (i * canvas.RenderSize.Height)/ (RowCount);
        }

        private void CanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.HeightChanged)
            {
                DrawLines();
                canvas.Children.Add(viewWindow);
                DrawViewRectangle();
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            int delta = e.Delta;
            // positive delta -> scroll up
            // negative delta -> scroll down

            this.VerticalOffset -= delta;
            if (this.VerticalOffset < 0)
            {
                this.VerticalOffset = 0;
            }
            else if (this.VerticalOffset > this.ScrollingLimit)
            {
                this.VerticalOffset = this.ScrollingLimit;
            }
            e.Handled = true;
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            Point p = Mouse.GetPosition(canvas);
            double newVerticalOffset = (((p.Y - this.viewWindow.ActualHeight / 2) * this.ScrollingLimit) / canvas.RenderSize.Height);
            VerticalOffset = newVerticalOffset;

        }
    }
}
