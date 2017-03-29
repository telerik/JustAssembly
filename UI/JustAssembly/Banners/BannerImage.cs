using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace JustAssembly.Banners
{
    public class BannerImage : Image
    {
        private Banner banner;

        public BannerImage()
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
            this.Cursor = Cursors.Hand;
        }

        public int HolderId { get; set; }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.banner = BannerService.Instance.GetBanner(this.HolderId);
            if (this.banner != null)
            {
                this.Source = this.banner.Image;
                this.MouseLeftButtonDown += OnMouseLeftButtonDown;
                this.Visibility = System.Windows.Visibility.Visible;
            }
        }

        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(banner.Link.Trim());
            }
            catch (Exception ex)
            {
                Configuration.ShowExceptionMessage(ex);
            }
        }
    }
}
