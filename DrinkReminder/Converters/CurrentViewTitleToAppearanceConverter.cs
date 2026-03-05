using System.Globalization;
using System.Windows.Data;

namespace DrinkReminder.Converters;

/// <summary>
/// Convert current view title to button appearance.
/// </summary>
public class CurrentViewTitleToAppearanceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string currentTitle && parameter is string tabTitle)
        {
            return string.Equals(currentTitle, tabTitle, StringComparison.Ordinal)
                ? "Primary"
                : "Transparent";
        }

        return "Transparent";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

