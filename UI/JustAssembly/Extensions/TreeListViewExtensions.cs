using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JustAssembly.Nodes;
using System.Windows.Controls;
using ICSharpCode.TreeView;

namespace JustAssembly.Extensions
{
    class TreeListViewExtensions
    {
        public static readonly DependencyProperty DoubleClickCommandProperty =
           DependencyProperty.RegisterAttached("DoubleClickCommand",
                                               typeof(ICommand),
                                               typeof(TreeListViewExtensions),
                                               new PropertyMetadata(OnDoubleClickCommandChanged));

        public static readonly DependencyProperty ShowAllUnmodifiedProperty =
            DependencyProperty.RegisterAttached("ShowAllUnmodified",
                                                typeof(bool),
                                                typeof(TreeListViewExtensions));
        
        private static void OnDoubleClickCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Border border = d as Border;
            if (border == null)
            {
                return;
            }

            border.MouseLeftButtonDown += OnTreeListViewExtensionsMouseDoubleClick;
        }

        private static void OnTreeListViewExtensionsMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                e.Handled = true;

                Border border = sender as Border;

                ICommand comamnd = GetDoubleClickCommand(border);
                if (comamnd != null)
                {
                    comamnd.Execute(((border.Child as StackPanel).Children[1] as SharpTreeNodeView).Node);
                }
            }
        }

        public static ICommand GetDoubleClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DoubleClickCommandProperty);
        }

        public static void SetDoubleClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DoubleClickCommandProperty, value);
        }

        public static bool GetShowAllUnmodified(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowAllUnmodifiedProperty);
        }

        public static void SetShowAllUnmodified(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowAllUnmodifiedProperty, value);
        }
    }
}
