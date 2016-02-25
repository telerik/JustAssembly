using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using JustAssembly.Interfaces;
using Dragablz;

namespace JustAssembly.Extensions
{
    public class TabExtensions
    {
        #region [Attached properties]
        private static readonly DependencyProperty AttachedObjectProperty =
            DependencyProperty.RegisterAttached("AttachedObject",
                                                typeof(TabablzControl),
                                                typeof(TabExtensions), null);

        public static readonly DependencyProperty ChangeSelectionOnKeyboardStrokeProperty =
            DependencyProperty.RegisterAttached("ChangeSelectionOnKeyboardStroke",
                                                typeof(bool),
                                                typeof(TabExtensions),
                                                new PropertyMetadata(OnChangeSelectionOnKeyboardStrokeChanged));

        public static readonly DependencyProperty ChangeSelectionOnMouseScrollProperty =
            DependencyProperty.RegisterAttached("ChangeSelectionOnMouseScroll",
                                                typeof(bool),
                                                typeof(TabExtensions),
                                                new PropertyMetadata(OnChangeSelectionOnMouseScrollChanged));

        public static readonly DependencyProperty CloseTabOnAltF4Property =
            DependencyProperty.RegisterAttached("CloseTabOnAltF4",
                                                typeof(bool),
                                                typeof(TabExtensions),
                                                new PropertyMetadata(OnCloseTabOnAltF4Changed));

        public static readonly DependencyProperty RemovedCurrentItemCommandProperty =
            DependencyProperty.RegisterAttached("RemovedCurrentItemCommand",
                                                typeof(ICommand),
                                                typeof(TabExtensions), null);

        public static readonly DependencyProperty AttachedCommandProperty =
          DependencyProperty.RegisterAttached("AttachedCommand",
                                              typeof(ICommand),
                                              typeof(TabExtensions), null);

        public static readonly DependencyProperty RoutedEventCommandProperty =
            DependencyProperty.RegisterAttached("RoutedEventCommand",
                                                typeof(RoutedEvent),
                                                typeof(TabExtensions),
                                                new PropertyMetadata(OnRoutedEventCommand));

        public static ICommand GetRemovedCurrentItemCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(RemovedCurrentItemCommandProperty);
        }

        public static void SetRemovedCurrentItemCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(RemovedCurrentItemCommandProperty, value);
        }

        public static bool GetCloseTabOnAltF4(DependencyObject obj)
        {
            return (bool)obj.GetValue(CloseTabOnAltF4Property);
        }

        public static void SetCloseTabOnAltF4(DependencyObject obj, bool value)
        {
            obj.SetValue(CloseTabOnAltF4Property, value);
        }

        public static bool GetChangeSelectionOnMouseScroll(DependencyObject obj)
        {
            return (bool)obj.GetValue(ChangeSelectionOnMouseScrollProperty);
        }

        public static void SetChangeSelectionOnMouseScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(ChangeSelectionOnMouseScrollProperty, value);
        }

        public static bool GetChangeSelectionOnKeyboardStroke(DependencyObject obj)
        {
            return (bool)obj.GetValue(ChangeSelectionOnKeyboardStrokeProperty);
        }

        public static void SetChangeSelectionOnKeyboardStroke(DependencyObject obj, bool value)
        {
            obj.SetValue(ChangeSelectionOnKeyboardStrokeProperty, value);
        }

        private static TabablzControl GetAttachedObject(DependencyObject obj)
        {
            return (TabablzControl)obj.GetValue(AttachedObjectProperty);
        }

        private static void SetAttachedObject(DependencyObject obj, Selector value)
        {
            obj.SetValue(AttachedObjectProperty, value);
        }

        public static ICommand GetAttachedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(AttachedCommandProperty);
        }

        public static void SetAttachedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(AttachedCommandProperty, value);
        }

        public static RoutedEvent GetRoutedEventCommand(DependencyObject obj)
        {
            return (RoutedEvent)obj.GetValue(RoutedEventCommandProperty);
        }

        public static void SetRoutedEventCommand(DependencyObject obj, RoutedEvent value)
        {
            obj.SetValue(RoutedEventCommandProperty, value);
        }
        #endregion

        private static void OnCloseTabOnAltF4Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabablzControl sender = d as TabablzControl;

            if (sender == null)
            {
                throw new ArgumentNullException("sender is not a Selector");
            }
            if ((bool)e.NewValue)
            {
                SetAttachedObject(Application.Current.MainWindow, sender);

                Application.Current.MainWindow.KeyDown += OnTabKeyDown;
            }
            else
            {
                Application.Current.MainWindow.ClearValue(AttachedObjectProperty);

                Application.Current.MainWindow.KeyDown -= OnTabKeyDown;
            }
        }

        private static void OnTabKeyDown(object sender, KeyEventArgs e)
        {
            TabablzControl tabablz = GetAttachedObject(Application.Current.MainWindow);

            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.F4))
            {
                ICommand removeCommand = GetRemovedCurrentItemCommand(tabablz);
                if (removeCommand != null)
                {
                    removeCommand.Execute(tabablz.SelectedItem);
                }
            }
        }

        private static void OnChangeSelectionOnMouseScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Selector sender = d as Selector;
            if (sender == null)
            {
                throw new ArgumentNullException("sender is not MultiSelector");
            }
            if ((bool)e.NewValue)
            {
                sender.Loaded += OnSenderLoaded;
                sender.Unloaded += OnSenderUnloaded;
            }
            else
            {
                sender.Loaded -= OnSenderLoaded;
                sender.Unloaded -= OnSenderUnloaded;
            }
        }

        private static void OnChangeSelectionOnKeyboardStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Selector sender = d as Selector;
            if (sender == null)
            {
                throw new ArgumentNullException("sender is not a Selector");
            }
            if ((bool)e.NewValue)
            {
                SetAttachedObject(Application.Current.MainWindow, sender);

                sender.PreviewKeyDown += OnSenderPreviewKeyDown;

                Application.Current.MainWindow.PreviewKeyDown += OnAppKeyDown;

                sender.Unloaded += OnSenderUnloaded;
            }
            else
            {
                Application.Current.MainWindow.ClearValue(AttachedObjectProperty);

                Application.Current.MainWindow.PreviewKeyDown -= OnAppKeyDown;

                sender.PreviewKeyDown += OnSenderPreviewKeyDown;

                sender.Unloaded -= OnSenderUnloaded;
            }
        }

        private static void OnSenderPreviewKeyDown(object sender, KeyEventArgs e)
        {
            Selector selector = sender as Selector;

            if (e.Key == Key.Right)
            {
                SetNewIndex(selector, 1);
            }
            else if (e.Key == Key.Left)
            {
                SetNewIndex(selector, -1);
            }
        }

        private static void OnAppKeyDown(object sender, KeyEventArgs e)
        {
            Selector selector = GetAttachedObject(Application.Current.MainWindow);

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (Keyboard.IsKeyDown(Key.Tab) && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                {
                    SetNewIndex(selector, -1);
                }
                else if (Keyboard.IsKeyDown(Key.Tab))
                {
                    SetNewIndex(selector, 1);
                }
                else
                {
                    //1 == 35 enum index
                    int index = (int)e.Key - 35 + 1;
                    if (index > 0 && index < 9)
                    {
                        index--;
                        if (index > -1 && index < selector.Items.Count)
                        {
                            selector.SelectedIndex = index;
                        }
                    }
                    else if (index == 9)
                    {
                        selector.SelectedIndex = selector.Items.Count - 1;
                    }
                }
            }
        }

        private static void OnSenderUnloaded(object sender, RoutedEventArgs e)
        {
            Control tabControl = sender as Control;

            ScrollViewer tabItemsParentScroll = tabControl.Template.FindName("ScrollViewerElement", tabControl) as ScrollViewer;

            tabItemsParentScroll.PreviewMouseWheel -= OnSenderMouseWheel;
        }

        private static void OnSenderLoaded(object sender, RoutedEventArgs e)
        {
        }

        private static void OnSenderMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Selector selector = ((Control)sender).TemplatedParent as Selector;

            int index = e.Delta > 0 ? 1 : -1;

            SetNewIndex(selector, index);
        }

        private static void SetNewIndex(Selector selector, int index)
        {
            if (selector.SelectedIndex + index > selector.Items.Count - 1)
            {
                selector.SelectedIndex = 0;
            }
            else if (selector.SelectedIndex + index < 0)
            {
                selector.SelectedIndex = selector.Items.Count - 1;
            }
            else
            {
                selector.SelectedIndex += index;
            }
        }

        private static void OnRoutedEventCommand(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement sender = d as FrameworkElement;

            if (sender == null)
            {
                return;
            }
            var oldValue = e.OldValue as RoutedEvent;
            if (oldValue != null)
            {
                sender.RemoveHandler(oldValue, new MouseButtonEventHandler(OnElementMouseDown));
            }
            var routedEvent = e.NewValue as RoutedEvent;
            if (routedEvent != null)
            {
                if (sender.IsLoaded)
                {
                    sender.AddHandler(routedEvent, new MouseButtonEventHandler(OnElementMouseDown));
                }
                sender.Unloaded += OnElementUnloaded;

                sender.Loaded += OnElementLoaded;
            }
        }

        static void OnElementUnloaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement item = sender as FrameworkElement;

            RoutedEvent routedEvent = GetRoutedEventCommand(item);

            if (routedEvent != null)
            {
                item.RemoveHandler(routedEvent, new MouseButtonEventHandler(OnElementMouseDown));
            }
            item.Unloaded -= OnElementUnloaded;
        }

        private static void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement item = sender as FrameworkElement;

            RoutedEvent routedEvent = GetRoutedEventCommand(item);

            item.AddHandler(routedEvent, new MouseButtonEventHandler(OnElementMouseDown));
        }

        static void OnElementMouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;

            if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
            {
                ICommand command = GetAttachedCommand(element);

                if (command == null)
                {
                    return;
                }
                command.Execute(element.DataContext);

                e.Handled = true;
            }
        }
    }
}
