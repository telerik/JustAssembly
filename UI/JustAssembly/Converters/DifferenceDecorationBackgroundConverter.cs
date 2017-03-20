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
                    return new SolidColorBrush(Configuration.ModifiedColor);
                case DifferenceDecoration.Deleted:
                    return new SolidColorBrush(Configuration.DeletedColor);
                case DifferenceDecoration.Added:
                    return new SolidColorBrush(Configuration.AddedColor);
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
