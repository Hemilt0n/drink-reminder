using System.Globalization;
using System.Windows.Data;

namespace DrinkReminder.Converters;

/// <summary>
/// 选中天数到按钮外观的转换器
/// </summary>
public class SelectedDaysToAppearanceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int selectedDays && parameter is string paramStr && int.TryParse(paramStr, out int paramDays))
        {
            return selectedDays == paramDays ? "Primary" : "Transparent";
        }
        return "Transparent";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}