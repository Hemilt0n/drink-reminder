using System.Globalization;
using System.Windows.Data;

namespace DrinkReminder.Converters;

/// <summary>
/// 进度转换为 StrokeDashArray 的转换器
/// </summary>
public class ProgressConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is double progress)
        {
            // 假设圆的周长为 100（实际大小由 StrokeThickness 决定）
            const double circumference = 100;

            // 计算弧长
            var arcLength = progress * circumference;
            var remainingLength = circumference - arcLength;

            return new System.Windows.Media.DoubleCollection(new[] { arcLength, remainingLength });
        }
        return new System.Windows.Media.DoubleCollection(new[] { 0.0, 100.0 });
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}