using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace JustAssembly
{
    public class ToolWindow : Window
    {
        public bool HideOnClose { get; set; }

        public event EventHandler BeforeHide = delegate { };

        public event EventHandler AfterHide = delegate { };

        private Action<ToolWindow> hideCallback = delegate { };

        private Action loadedCallback = delegate { };

        public static void Show(IWindowView content,
                                Action<ToolWindow> hideCallback = null,
                                Action loadedCallback = null,
                                double width = 400,
                                double height = 300,
                                ResizeMode resizeMode = ResizeMode.NoResize,
                                bool showInTaskBar = false,
                                WindowStartupLocation startupLocation = WindowStartupLocation.CenterOwner)
        {
            ShowWindow(content, hideCallback, loadedCallback, width, height, resizeMode, showInTaskBar, startupLocation, false);
        }

        public static void ShowDialog(IWindowView content,
                                    Action<ToolWindow> hideCallback = null,
                                    Action loadedCallback = null,
                                    double width = 400,
                                    double height = 300,
                                    ResizeMode resizeMode = ResizeMode.NoResize,
                                    bool showInTaskBar = false,
                                    WindowStartupLocation startupLocation = WindowStartupLocation.CenterOwner,
                                    WindowStyle style = WindowStyle.SingleBorderWindow)
        {
            ShowWindow(content, hideCallback, loadedCallback, width, height, resizeMode, showInTaskBar, startupLocation, true, style);
        }

        private static void ShowWindow(IWindowView content,
                                       Action<ToolWindow> hideCallback = null,
                                       Action loadedCallback = null,
                                       double width = 400,
                                       double height = 300,
                                       ResizeMode resizeMode = ResizeMode.NoResize,
                                       bool showInTaskBar = false,
                                       WindowStartupLocation startupLocation = WindowStartupLocation.CenterOwner,
                                       bool isModal = false,
                                       WindowStyle style = WindowStyle.SingleBorderWindow)
        {
            if (Application.Current.MainWindow == null || !Application.Current.MainWindow.IsLoaded)
            {
                DispatcherObjectExt.BeginInvoke(() => ShowWindow(content, hideCallback, loadedCallback, width, height, resizeMode, showInTaskBar, startupLocation, isModal, style), DispatcherPriority.Background);

                return;
            }
            var toolWindow = new ToolWindow
            {
                Width = width,
                Height = height,
                Content = content,
                ResizeMode = resizeMode,
                ShowInTaskbar = showInTaskBar,
                WindowStartupLocation = startupLocation,
                Owner = Application.Current.MainWindow,
                hideCallback = hideCallback,
                loadedCallback = loadedCallback,
                WindowStyle = style,
            };
            toolWindow.Loaded += OnLoaded;

            toolWindow.AfterHide += OnAfterHide;

            if (isModal)
            {
                toolWindow.ShowDialog();
            }
            else
            {
                toolWindow.Show();
            }
        }

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            var toolWindow = sender as ToolWindow;
            if (toolWindow.loadedCallback != null)
            {
                toolWindow.loadedCallback();
                toolWindow.loadedCallback = null;
            }
            toolWindow.Loaded -= OnLoaded;

            ((IWindowView)toolWindow.Content).OnShowWindow();
        }

        public ToolWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            this.ShowInTaskbar = false;

            this.HideOnClose = true;

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseCommandHandler));
        }
        
        protected override void OnActivated(EventArgs e)
        {
            var windowView = this.Content as IWindowView;
            if (windowView != null)
            {
                if (!string.IsNullOrEmpty(windowView.Title))
                {
                    this.Title = windowView.Title;
                }
                if (windowView.Icon != null)
                {
                    this.Icon = windowView.Icon;
                }
                DispatcherObjectExt.BeginInvoke(() => windowView.TrySetFocus());
                windowView.SendCloseRequest += OnWindowViewCloseRequest;
            }
            base.OnActivated(e);
        }

        private void OnWindowViewCloseRequest(object sender, EventArgs e)
        {
            var windowView = sender as IWindowView;

            this.Close();

            windowView.SendCloseRequest -= OnWindowViewCloseRequest;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (HideOnClose)
            {
                BeforeHide(this, EventArgs.Empty);
                base.Hide();
                e.Cancel = true;
                AfterHide(this, EventArgs.Empty);
            }
            else
            {
                base.OnClosing(e);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
                return;
            }
            base.OnKeyDown(e);
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            Application.Current.MainWindow.Focus();

            base.OnClosed(e);
        }

        private static void OnAfterHide(object sender, EventArgs e)
        {
            var toolWindow = sender as ToolWindow;
            if (toolWindow.hideCallback != null)
            {
                toolWindow.hideCallback(toolWindow);
            }
            toolWindow.hideCallback = null;
            toolWindow.AfterHide -= OnAfterHide;

            var toolContent = toolWindow.Content as WindowViewBase;
            if (toolContent != null)
            {
                toolContent.Close();
            }
            toolWindow.Content = null;
            toolWindow = null;

            Application.Current.MainWindow.Focus();
        }
    }
}