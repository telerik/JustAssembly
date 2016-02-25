using System;
using System.Collections.Generic;
using JustAssembly.Core;

namespace JustAssembly.Nodes.APIDiff
{
    public class APIDiffInfo
    {
        public IMetadataDiffItem APIDiffItem { get; private set; }

        public APIDiffInfo(IMetadataDiffItem apiDiffItem)
        {
            this.APIDiffItem = apiDiffItem;
        }
    }
}
