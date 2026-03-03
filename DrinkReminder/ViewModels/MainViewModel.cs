using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrinkReminder.Helpers;
using DrinkReminder.Models;
using DrinkReminder.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace DrinkReminder.ViewModels;

/// <summary>
/// 主窗口视图模型
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly TrayService _trayService;
    private readonly ReminderService _reminderService;

    [ObservableProperty]
    private ObservableObject _currentView;

    [ObservableProperty]
    private string _currentViewTitle = "首页";

    [ObservableProperty]
    private DailyStats _todayStats = new();

    [ObservableProperty]
    private AppSettings _settings = new();

    public HomeViewModel HomeViewModel { get; }
    public HistoryViewModel HistoryViewModel { get; }
    public SettingsViewModel SettingsViewModel { get; }

    public MainViewModel(DatabaseService databaseService, TrayService trayService, ReminderService reminderService)
    {
        _databaseService = databaseService;
        _trayService = trayService;
        _reminderService = reminderService;

        // 加载设置
        Settings = _databaseService.GetSettings();
        ThemeHelper.ApplyTheme(Settings.Theme);
        AutoStartHelper.UpdateAutoStart(Settings.AutoStart);

        // 初始化子视图模型
        HomeViewModel = new HomeViewModel(databaseService, this);
        HistoryViewModel = new HistoryViewModel(databaseService);
        SettingsViewModel = new SettingsViewModel(databaseService, this);

        // 设置默认视图
        _currentView = HomeViewModel;

        // 订阅托盘服务事件
        _trayService.QuickRecordRequested += OnQuickRecord;
        _trayService.ShowMainWindowRequested += OnShowMainWindow;
        _trayService.ExitRequested += OnExit;

        // 订阅提醒服务事件
        _reminderService.ReminderTriggered += OnReminderTriggered;

        // 加载今日数据
        RefreshTodayStats();
    }

    /// <summary>
    /// 刷新今日统计数据
    /// </summary>
    public void RefreshTodayStats()
    {
        TodayStats = _databaseService.GetTodayStats();
        _trayService.UpdateProgress(TodayStats.TotalMl, TodayStats.GoalMl);
    }

    /// <summary>
    /// 导航到首页
    /// </summary>
    [RelayCommand]
    private void NavigateToHome()
    {
        CurrentView = HomeViewModel;
        CurrentViewTitle = "首页";
    }

    /// <summary>
    /// 导航到历史记录页
    /// </summary>
    [RelayCommand]
    private void NavigateToHistory()
    {
        CurrentView = HistoryViewModel;
        CurrentViewTitle = "统计";
    }

    /// <summary>
    /// 导航到设置页
    /// </summary>
    [RelayCommand]
    private void NavigateToSettings()
    {
        CurrentView = SettingsViewModel;
        CurrentViewTitle = "设置";
    }

    /// <summary>
    /// 添加饮水记录
    /// </summary>
    public void AddDrinkRecord(int amountMl, string? note = null)
    {
        _databaseService.AddRecord(amountMl, note);
        RefreshTodayStats();

        // 重置提醒计时器
        _reminderService.ResetTimer();
    }

    /// <summary>
    /// 更新设置
    /// </summary>
    public void UpdateSettings(AppSettings newSettings)
    {
        _databaseService.SaveSettings(newSettings);
        Settings = newSettings;
        AutoStartHelper.UpdateAutoStart(newSettings.AutoStart);
        ThemeHelper.ApplyTheme(newSettings.Theme);
        _reminderService.UpdateSettings(newSettings);
        _trayService.UpdateProgress(TodayStats.TotalMl, Settings.DailyGoalMl);
        _trayService.UpdateQuickRecordMenu(newSettings.QuickRecordButtons);
        HomeViewModel.RefreshData();
        HistoryViewModel.RefreshData();
    }

    private void OnQuickRecord(int amountMl)
    {
        AddDrinkRecord(amountMl);
        _trayService.ShowBalloonTip("记录成功", $"已记录 {amountMl} ml 饮水");
    }

    private void OnShowMainWindow()
    {
        // 请求显示主窗口（由 MainWindow 处理）
        ShowMainWindowRequested?.Invoke();
    }

    private void OnExit()
    {
        ExitRequested?.Invoke();
    }

    private void OnReminderTriggered(object? sender, ReminderEventArgs e)
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher == null || dispatcher.CheckAccess())
        {
            _trayService.ShowBalloonTip("💧 喝水提醒", e.Message);
            return;
        }

        dispatcher.Invoke(() => _trayService.ShowBalloonTip("💧 喝水提醒", e.Message));
    }

    /// <summary>
    /// 显示主窗口请求事件
    /// </summary>
    public event Action? ShowMainWindowRequested;

    /// <summary>
    /// 退出应用请求事件
    /// </summary>
    public event Action? ExitRequested;
}
