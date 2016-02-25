using JustAssembly.Nodes;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace JustAssembly.Converters
{
    public class DifferenceDecorationBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DifferenceDecoration decoration = (DifferenceDecoration)value;
            switch (decoration)
            {
                case DifferenceDecoration.NoDifferences:
                    return new SolidColorBrush(Colors.Transparent);
                case DifferenceDecoration.Modified:
                    return new SolidColorBrush(Color.FromRgb(240, 240, 255));
                case DifferenceDecoration.Deleted:
                    return new SolidColorBrush(Color.FromRgb(255, 221, 221));
                case DifferenceDecoration.Added:
                    return new SolidColorBrush(Color.FromRgb(221, 255, 221));
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
