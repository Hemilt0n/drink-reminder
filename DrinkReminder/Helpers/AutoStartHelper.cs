using Microsoft.Win32;
using System;
using System.IO;

namespace DrinkReminder.Helpers;

/// <summary>
/// 自启动管理辅助类
/// </summary>
public static class AutoStartHelper
{
    private const string AppName = "DrinkReminder";
    private static readonly string AppPath = Environment.ProcessPath ?? "";

    /// <summary>
    /// 设置开机自启动
    /// </summary>
    public static void EnableAutoStart()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run", true);

            key?.SetValue(AppName, $"\"{AppPath}\" --minimized");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to enable auto-start: {ex.Message}");
        }
    }

    /// <summary>
    /// 禁用开机自启动
    /// </summary>
    public static void DisableAutoStart()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run", true);

            key?.DeleteValue(AppName, false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to disable auto-start: {ex.Message}");
        }
    }

    /// <summary>
    /// 检查是否已设置自启动
    /// </summary>
    public static bool IsAutoStartEnabled()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run", false);

            var value = key?.GetValue(AppName) as string;
            return !string.IsNullOrEmpty(value);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 更新自启动设置
    /// </summary>
    public static void UpdateAutoStart(bool enable)
    {
        if (enable)
        {
            EnableAutoStart();
        }
        else
        {
            DisableAutoStart();
        }
    }
}