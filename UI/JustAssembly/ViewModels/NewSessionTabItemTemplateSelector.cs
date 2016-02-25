using System.Windows;
using System.Windows.Controls;
using JustAssembly.Interfaces;
using JustAssembly.SelectorControl;

namespace JustAssembly.ViewModels
{
    public class NewSessionTabItemTemplateSelector : DataTemplateSelector
    {
        public NewSessionTabItemTemplateSelector() { }

        public DataTemplate AssembliesBrowseTemplate { get; set; }

        public DataTemplate DirectoriesBrowseTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            IComparisonSessionModel tabSourceItem = item as IComparisonSessionModel;
            if (tabSourceItem == null)
            {
                return base.SelectTemplate(item, container);
            }
            switch (tabSourceItem.SelectedItemType)
            {
                case SelectedItemType.File:
                    return this.AssembliesBrowseTemplate;

                case SelectedItemType.Folder:
                    return this.DirectoriesBrowseTemplate;
                      
                default:
                    return base.SelectTemplate(item, container);
            }
        }
    }
}