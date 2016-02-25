using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace JustAssembly
{
    public partial class NewSessionDialog : Window
    {
        public NewSessionDialog()
        {
            InitializeComponent();

            this.ShowInTaskbar = false;

            this.ResizeMode = ResizeMode.NoResize;

            this.WindowStyle = WindowStyle.ToolWindow;

            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            this.Owner = Application.Current.MainWindow;

            this.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;

                this.Close();
            }
        }

        public bool ShowModal()
        {
            return this.ShowDialog() == true;
        }

        private void OnLoadClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;

            this.Close();
        }
    }
}
