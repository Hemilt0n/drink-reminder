using Wpf.Ui.Appearance;

namespace DrinkReminder.Helpers;

/// <summary>
/// 主题切换辅助类
/// </summary>
public static class ThemeHelper
{
    /// <summary>
    /// 应用主题
    /// </summary>
    /// <param name="theme">主题名称: light, dark, system</param>
    public static void ApplyTheme(string theme)
    {
        var actualTheme = theme.ToLower() switch
        {
            "light" => ApplicationTheme.Light,
            "dark" => ApplicationTheme.Dark,
            _ => GetSystemTheme() // system 或默认跟随系统
        };

        ApplicationThemeManager.Apply(actualTheme);
    }

    /// <summary>
    /// 获取系统当前主题
    /// </summary>
    private static ApplicationTheme GetSystemTheme()
    {
        // Windows 10/11 系统主题检测
        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");

            if (key?.GetValue("AppsUseLightTheme") is int value)
            {
                return value == 0 ? ApplicationTheme.Dark : ApplicationTheme.Light;
            }
        }
        catch
        {
            // 无法读取注册表时默认使用浅色主题
        }

        return ApplicationTheme.Light;
    }

    /// <summary>
    /// 获取当前应用主题
    /// </summary>
    public static ApplicationTheme GetCurrentTheme()
    {
        return ApplicationThemeManager.GetAppTheme();
    }
}