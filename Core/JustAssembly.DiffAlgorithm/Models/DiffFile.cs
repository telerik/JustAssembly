using System.Collections.Generic;

namespace JustAssembly.DiffAlgorithm.Models
{
    public class DiffFile
    {
        public IList<DiffBlock> Blocks { get; set; }
        public IList<string> Lines { get; set; }
        public DiffFile()
            : this(new List<string>(), new List<DiffBlock>())
        {
        }

        public DiffFile(IList<string> lines, IList<DiffBlock> blocks)
        {
            this.Lines = lines;
            this.Blocks = blocks;
        }
    }
}
