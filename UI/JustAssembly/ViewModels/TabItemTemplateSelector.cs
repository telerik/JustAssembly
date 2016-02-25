using System.Windows;
using System.Windows.Controls;
using JustAssembly.Interfaces;

namespace JustAssembly.ViewModels
{
    public class TabItemTemplateSelector : DataTemplateSelector
    {
        public TabItemTemplateSelector() { }

        public DataTemplate AssemblyBrowseTemplate { get; set; }

        public DataTemplate JustAssemblyTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ITabSourceItem tabSourceItem = item as ITabSourceItem;
            if (tabSourceItem == null)
            {
                return base.SelectTemplate(item, container);
            }
            switch (tabSourceItem.TabKind)
            {
                case TabKind.DirectoryBrowse:
                case TabKind.AssemblyBrowse:
                    return this.AssemblyBrowseTemplate;

                case TabKind.JustAssembly:
                    return this.JustAssemblyTemplate;
                    
                default:
                    return base.SelectTemplate(item, container);
            }
        }
    }
}