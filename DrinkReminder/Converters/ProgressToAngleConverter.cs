using System.Globalization;
using System.Windows.Data;

namespace DrinkReminder.Converters;

/// <summary>
/// 进度转换为角度转换器（用于进度环）
/// </summary>
public class ProgressToAngleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double progress)
        {
            // 计算圆弧角度（0-360度）
            return progress * 360;
        }
        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}