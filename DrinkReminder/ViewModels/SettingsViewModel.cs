using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrinkReminder.Models;
using DrinkReminder.Services;
using System.Collections.ObjectModel;

namespace DrinkReminder.ViewModels;

/// <summary>
/// Settings view model.
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
    private string _selectedTheme = "system";

    [ObservableProperty]
    private ObservableCollection<int> _quickRecordButtons = new();

    [ObservableProperty]
    private int _newQuickAmount = 200;

    [ObservableProperty]
    private bool _showAddQuickButtonDialog;

    public IReadOnlyList<TimeSpan> ReminderTimeOptions { get; } =
        Enumerable.Range(0, 24).Select(h => new TimeSpan(h, 0, 0)).ToList();

    public SettingsViewModel(DatabaseService databaseService, MainViewModel mainViewModel)
    {
        _databaseService = databaseService;
        _mainViewModel = mainViewModel;

        LoadSettings();
    }

    /// <summary>
    /// Load settings from model.
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
        SelectedTheme = NormalizeTheme(settings.Theme);

        QuickRecordButtons.Clear();
        foreach (var amount in settings.QuickRecordButtons.OrderBy(x => x))
        {
            QuickRecordButtons.Add(amount);
        }
    }

    /// <summary>
    /// Persist settings.
    /// </summary>
    [RelayCommand]
    private void SaveSettings()
    {
        var sanitizedButtons = QuickRecordButtons
            .Where(x => x > 0)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var settings = new AppSettings
        {
            DailyGoalMl = Math.Clamp(DailyGoalMl, 500, 5000),
            ReminderIntervalMinutes = Math.Clamp(ReminderIntervalMinutes, 5, 120),
            ReminderEnabled = ReminderEnabled,
            ReminderStartTime = ReminderStartTime,
            ReminderEndTime = ReminderEndTime,
            AutoStart = AutoStart,
            MinimizeToTray = MinimizeToTray,
            CloseToTray = CloseToTray,
            Theme = NormalizeTheme(SelectedTheme),
            QuickRecordButtons = sanitizedButtons
        };

        _mainViewModel.UpdateSettings(settings);
    }

    /// <summary>
    /// Open add quick button dialog.
    /// </summary>
    [RelayCommand]
    private void AddQuickRecordButton()
    {
        NewQuickAmount = 200;
        ShowAddQuickButtonDialog = true;
    }

    /// <summary>
    /// Confirm add quick button.
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
    /// Cancel add quick button.
    /// </summary>
    [RelayCommand]
    private void CancelAddQuickButton()
    {
        ShowAddQuickButtonDialog = false;
    }

    /// <summary>
    /// Remove quick button.
    /// </summary>
    [RelayCommand]
    private void RemoveQuickRecordButton(int amount)
    {
        QuickRecordButtons.Remove(amount);
        SaveSettings();
    }

    /// <summary>
    /// Clear all drink records.
    /// </summary>
    [RelayCommand]
    private void ClearAllData()
    {
        _databaseService.ClearAllData();
        _mainViewModel.RefreshTodayStats();
        _mainViewModel.HomeViewModel.RefreshData();
        _mainViewModel.HistoryViewModel.RefreshData();
    }

    /// <summary>
    /// Export records.
    /// </summary>
    [RelayCommand]
    private void ExportData()
    {
        // TODO: CSV export.
    }

    /// <summary>
    /// Reset settings to defaults.
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
        SelectedTheme = NormalizeTheme(defaultSettings.Theme);

        QuickRecordButtons.Clear();
        foreach (var amount in defaultSettings.QuickRecordButtons.OrderBy(x => x))
        {
            QuickRecordButtons.Add(amount);
        }

        SaveSettings();
    }

    private static string NormalizeTheme(string? theme)
    {
        return theme?.Trim().ToLowerInvariant() switch
        {
            "light" => "light",
            "dark" => "dark",
            _ => "system"
        };
    }
}

