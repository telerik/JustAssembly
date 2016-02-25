using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JustAssembly.Infrastructure.CodeViewer
{
    public class DiffBackgroundRenderer : IBackgroundRenderer
    {
        private readonly Dictionary<int, ClassificationType> lineToClasificationTypeMap;

        public DiffBackgroundRenderer(Dictionary<int, ClassificationType> lineToClasificationTypeMap)
        {
            this.lineToClasificationTypeMap = lineToClasificationTypeMap;
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Background; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            textView.EnsureVisualLines();
            foreach (VisualLine line in textView.VisualLines)
            {
                int lineNumber = line.FirstDocumentLine.LineNumber - 1;
                if (this.lineToClasificationTypeMap.ContainsKey(lineNumber))
                {
                    Color color = GetColorFromClassificationType(this.lineToClasificationTypeMap[lineNumber]);
                    foreach (Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, new TextSegment() { StartOffset = line.FirstDocumentLine.Offset }))
                    {
                        Point start = new Point(r.Location.X + textView.ScrollOffset.X, r.Location.Y);
                        drawingContext.DrawRectangle(
                            new SolidColorBrush(color),
                            new Pen(new SolidColorBrush(color), 1),
                            new Rect(start, new Size(textView.ActualWidth, r.Height))
                        );
                    }
                }
            }
        }

        public static Color GetColorFromClassificationType(ClassificationType type)
        {
            switch (type)
            {
                case ClassificationType.ModifiedLine:
                    return Colors.LightBlue;
                case ClassificationType.InsertedLine:
                    return Colors.LightGreen;
                case ClassificationType.DeletedLine:
                    return Colors.Red;
                case ClassificationType.ImaginaryLine:
                    return Colors.LightGray;
                case ClassificationType.NotModifiedLine:
                    throw new ArgumentException("The not modified lines doesn't have color representation.");
                default:
                    throw new ArgumentException("Invalid classification type.");
            }
        }
    }
}
