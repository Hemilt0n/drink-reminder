using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrinkReminder.Models;
using DrinkReminder.Services;
using System.Collections.ObjectModel;

namespace DrinkReminder.ViewModels;

/// <summary>
/// 设置视图模型
/// </summary>
public partial class SettingsViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly MainViewModel _mainViewModel;

    [ObservableProperty]
    private int _dailyGoalMl;

    [ObservableProperty]
    private int _reminderIntervalMinutes;

    [ObservableProperty]
    private bool _reminderEnabled;

    [ObservableProperty]
    private TimeSpan _reminderStartTime;

    [ObservableProperty]
    private TimeSpan _reminderEndTime;

    [ObservableProperty]
    private bool _autoStart;

    [ObservableProperty]
    private bool _minimizeToTray;

    [ObservableProperty]
    private bool _closeToTray;

    [ObservableProperty]
    private string _selectedTheme;

    [ObservableProperty]
    private ObservableCollection<int> _quickRecordButtons = new();

    [ObservableProperty]
    private int _newQuickAmount = 100;

    [ObservableProperty]
    private bool _showAddQuickButtonDialog;

    public SettingsViewModel(DatabaseService databaseService, MainViewModel mainViewModel)
    {
        _databaseService = databaseService;
        _mainViewModel = mainViewModel;

        LoadSettings();
    }

    /// <summary>
    /// 加载设置
    /// </summary>
    public void LoadSettings()
    {
        var settings = _mainViewModel.Settings;

        DailyGoalMl = settings.DailyGoalMl;
        ReminderIntervalMinutes = settings.ReminderIntervalMinutes;
        ReminderEnabled = settings.ReminderEnabled;
        ReminderStartTime = settings.ReminderStartTime;
        ReminderEndTime = settings.ReminderEndTime;
        AutoStart = settings.AutoStart;
        MinimizeToTray = settings.MinimizeToTray;
        CloseToTray = settings.CloseToTray;
        SelectedTheme = settings.Theme;

        QuickRecordButtons.Clear();
        foreach (var amount in settings.QuickRecordButtons)
        {
            QuickRecordButtons.Add(amount);
        }
    }

    /// <summary>
    /// 保存设置
    /// </summary>
    [RelayCommand]
    private void SaveSettings()
    {
        var settings = new AppSettings
        {
            DailyGoalMl = DailyGoalMl,
            ReminderIntervalMinutes = ReminderIntervalMinutes,
            ReminderEnabled = ReminderEnabled,
            ReminderStartTime = ReminderStartTime,
            ReminderEndTime = ReminderEndTime,
            AutoStart = AutoStart,
            MinimizeToTray = MinimizeToTray,
            CloseToTray = CloseToTray,
            Theme = SelectedTheme,
            QuickRecordButtons = QuickRecordButtons.ToList()
        };

        _mainViewModel.UpdateSettings(settings);
    }

    /// <summary>
    /// 添加快捷记录按钮
    /// </summary>
    [RelayCommand]
    private void AddQuickRecordButton()
    {
        NewQuickAmount = 100;
        ShowAddQuickButtonDialog = true;
    }

    /// <summary>
    /// 确认添加快捷按钮
    /// </summary>
    [RelayCommand]
    private void ConfirmAddQuickButton()
    {
        if (NewQuickAmount > 0 && !QuickRecordButtons.Contains(NewQuickAmount))
        {
            QuickRecordButtons.Add(NewQuickAmount);
            SaveSettings();
        }
        ShowAddQuickButtonDialog = false;
    }

    /// <summary>
    /// 取消添加快捷按钮
    /// </summary>
    [RelayCommand]
    private void CancelAddQuickButton()
    {
        ShowAddQuickButtonDialog = false;
    }

    /// <summary>
    /// 删除快捷记录按钮
    /// </summary>
    [RelayCommand]
    private void RemoveQuickRecordButton(int amount)
    {
        QuickRecordButtons.Remove(amount);
        SaveSettings();
    }

    /// <summary>
    /// 清除所有数据
    /// </summary>
    [RelayCommand]
    private void ClearAllData()
    {
        // TODO: 显示确认对话框
        _databaseService.ClearAllData();
        _mainViewModel.RefreshTodayStats();
    }

    /// <summary>
    /// 导出数据
    /// </summary>
    [RelayCommand]
    private void ExportData()
    {
        // TODO: 实现 CSV 导出
    }

    /// <summary>
    /// 重置为默认设置
    /// </summary>
    [RelayCommand]
    private void ResetToDefault()
    {
        var defaultSettings = new AppSettings();
        DailyGoalMl = defaultSettings.DailyGoalMl;
        ReminderIntervalMinutes = defaultSettings.ReminderIntervalMinutes;
        ReminderEnabled = defaultSettings.ReminderEnabled;
        ReminderStartTime = defaultSettings.ReminderStartTime;
        ReminderEndTime = defaultSettings.ReminderEndTime;
        AutoStart = defaultSettings.AutoStart;
        MinimizeToTray = defaultSettings.MinimizeToTray;
        CloseToTray = defaultSettings.CloseToTray;
        SelectedTheme = defaultSettings.Theme;
        QuickRecordButtons.Clear();
        foreach (var amount in defaultSettings.QuickRecordButtons)
        {
            QuickRecordButtons.Add(amount);
        }

        SaveSettings();
    }
}