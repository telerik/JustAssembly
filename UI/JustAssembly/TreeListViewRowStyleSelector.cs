using System.Windows;
using System.Windows.Controls;
using JustAssembly.Nodes;

namespace JustAssembly
{
    public class TreeListViewRowStyleSelector : StyleSelector 
    {
        public Style AddedStyle { get; set; }

        public Style DeletedStyle { get; set; }

        public Style ModifiedStyle { get; set; }

        public Style NoDifferences { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is ItemNodeBase)
            {
                ItemNodeBase node = item as ItemNodeBase;
                switch (node.DifferenceDecoration)
                {
                    case DifferenceDecoration.Modified:
                        return ModifiedStyle;
                    case DifferenceDecoration.Deleted:
                        return DeletedStyle;
                    case DifferenceDecoration.Added:
                        return AddedStyle;
                    case DifferenceDecoration.NoDifferences:
                        return NoDifferences;
                    default:
                        return base.SelectStyle(item, container);
                }
            }
            return base.SelectStyle(item, container);
        }
    }
}