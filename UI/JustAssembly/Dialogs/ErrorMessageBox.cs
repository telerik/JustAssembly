using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JustAssembly
{
    public class ErrorMessageBox :  TextBox
    {
        public ErrorMessageBox(string text)
        {
            this.Text = text;
            this.BorderBrush = new SolidColorBrush(Colors.Transparent);
            this.BorderThickness = new Thickness();
            this.IsReadOnly = true;
            this.TextWrapping = TextWrapping.Wrap;
        }
    }
}
