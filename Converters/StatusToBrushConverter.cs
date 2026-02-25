using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using ViveGui.Models;

namespace ViveGui.Converters;

public class StatusToBrushConverter : IValueConverter
{
    private const double BRUSH_OPACITY = 0.25;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value switch
        {
            RowStatus.Configured => CreateBrush(Colors.Green),
            RowStatus.Error => CreateBrush(Colors.Red),
            RowStatus.Confirming => CreateBrush(Colors.Gold),
            RowStatus.Skipped => CreateBrush(Colors.Gray),
            _ => Brushes.Transparent
        };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    private static SolidColorBrush CreateBrush(Color baseColor)
    {
        var brush = new SolidColorBrush(baseColor)
        {
            Opacity = BRUSH_OPACITY
        };

        if (brush.CanFreeze)
            brush.Freeze();

        return brush;
    }
}
