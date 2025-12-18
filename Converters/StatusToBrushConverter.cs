using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ViveGui.Models;

namespace ViveGui.Converters;

public class StatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is RowStatus status)
        {
            return status switch
            {
                RowStatus.Configured => Brushes.LightGreen,
                RowStatus.Error => Brushes.LightPink,
                RowStatus.Confirming => Brushes.LightYellow,
                RowStatus.Skipped => Brushes.LightGray,
                _ => Brushes.Transparent
            };
        }
        return Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}