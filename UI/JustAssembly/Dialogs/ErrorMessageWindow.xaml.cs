using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Text;
using System.Diagnostics;
using System.Windows.Threading;

namespace JustAssembly
{
    public partial class ErrorMessageWindow : WindowViewBase
    {
        public ErrorMessageWindow()
        {
            InitializeComponent();
        }

        public ErrorMessageWindow(string errorMessage, string errorCaption = "")
            : this()
        {
            if (!string.IsNullOrWhiteSpace(errorCaption))
            {
                txtErrorCaption.Text = errorCaption;
            }
            txtErrorMessage.Text = errorMessage;
        }

        public override ImageSource Icon
        {
            get
            {
                return BitmapFrame.Create(new Uri("pack://application:,,,/JustAssembly;component/Images/Common/error_small.png", UriKind.RelativeOrAbsolute));
            }
        }

        public override string Title
        {
            get { return "Error"; }
        }

        private void OnCopyToClipBoardButtonClicked(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtErrorMessage.Text);
        }

        private void OnOKButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnLnkMailToTelerikClicked(object sender, RoutedEventArgs e)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("mailto:JustDecompilePublicFeedback@telerik.com?subject=Exception error");

            DispatcherObjectExt.BeginInvoke(() =>
            {
                try
                {
                    Process.Start(stringBuilder.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }, DispatcherPriority.Background);
        }
    }
}
