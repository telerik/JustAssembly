using System.Windows.Media;

namespace JustAssembly.Banners
{
    public class Banner
    {
        public Banner(ImageSource image, string link)
        {
            this.Image = image;
            this.Link = link;
        }

        public ImageSource Image { get; private set; }

        public string Link { get; private set; }
    }
}
