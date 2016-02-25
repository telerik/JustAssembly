using JustDecompile.External.JustAssembly;
using JustAssembly.Infrastructure.CodeViewer;

namespace JustAssembly
{
    public class Position : ISpan
    {
        public Position()
            : this(0, 0, -1) { }

        public Position(int start, int length, int lineNumber)
        {
            this.Length = length;

            this.Start = start;

            this.End = start + length;

            this.LineNumber = lineNumber;
        }

        public static Position Empty = new Position();

        public int Length { get; private set; }

        public int Start { get; private set; }

        public int End { get; private set; }

        public int LineNumber { get; private set; }
    }
}