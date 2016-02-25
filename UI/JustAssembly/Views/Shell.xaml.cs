using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JustAssembly.ViewModels;

namespace JustAssembly
{
    public partial class Shell : Window
    {
        private readonly IShellViewModel shellViewModel;

        public Shell(IShellViewModel viewModel)
        {
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            InitializeComponent();

            this.DataContext = this.shellViewModel = viewModel;

            this.Loaded += OnLoaded;

            this.PreviewKeyDown += OnMainWindowKeyDown;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.shellViewModel.OpenNewSessionCommandExecuted();
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
