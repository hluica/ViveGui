using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using ViveGui.Models;

namespace ViveGui.Converters;

public class StatusToBrushConverter : IValueConverter
{
    private const double BrushOpacity = 0.25;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is RowStatus status
            ? status switch
            {
                RowStatus.Configured => CreateBrush(Colors.Green),
                RowStatus.Error => CreateBrush(Colors.Red),
                RowStatus.Confirming => CreateBrush(Colors.Gold),
                RowStatus.Skipped => CreateBrush(Colors.Gray),
                _ => Brushes.Transparent
            }
            : Brushes.Transparent;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    /// <summary>
    /// 创建一个基于指定颜色但带有透明度的冻结画笔
    /// </summary>
    private static SolidColorBrush CreateBrush(Color baseColor)
    {
        var brush = new SolidColorBrush(baseColor)
        {
            Opacity = BrushOpacity
        };

        if (brush.CanFreeze)
            brush.Freeze();

        return brush;
    }
}
