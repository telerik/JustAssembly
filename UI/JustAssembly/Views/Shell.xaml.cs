using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JustAssembly.ViewModels;

namespace JustAssembly
{

    public partial class Shell : Window
    {
        private readonly IShellViewModel shellViewModel;
        private readonly string[] args;

        public Shell(IShellViewModel viewModel, string[] args)
        {
            this.args = args;
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            InitializeComponent();

            this.DataContext = this.shellViewModel = viewModel;

            this.Loaded += OnLoaded;

            this.PreviewKeyDown += OnMainWindowKeyDown;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.args?.Length == 0)
            {
                this.shellViewModel.OpenNewSessionCommandExecuted();
            }
            else
            {
                this.shellViewModel.OpenNewSessionWithCmdLineArgsCommandExecuted();
            }
        }

        private void OnMainWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.shellViewModel.CancelCurrentOperation();
            }
        }
    }
}
