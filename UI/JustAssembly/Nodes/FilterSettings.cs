using System;

namespace JustAssembly.Nodes
{
    public class FilterSettings
    {
        public FilterSettings(bool showUnmodified)
        {
            this.ShowUnmodified = showUnmodified;
        }

        public bool ShowUnmodified { get; set; }
    }
}