using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrinkReminder.Models;
using DrinkReminder.Services;
using System.Collections.ObjectModel;

namespace DrinkReminder.ViewModels;

/// <summary>
/// 首页视图模型
/// </summary>
public partial class HomeViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly MainViewModel _mainViewModel;

    [ObservableProperty]
    private DailyStats _todayStats = new();

    [ObservableProperty]
    private int _selectedQuickAmount = 200;

    [ObservableProperty]
    private bool _isCustomDialogOpen;

    [ObservableProperty]
    private int _customAmount = 200;

    [ObservableProperty]
    private string _customNote = string.Empty;

    public ObservableCollection<int> QuickAmounts { get; }

    public HomeViewModel(DatabaseService databaseService, MainViewModel mainViewModel)
    {
        _databaseService = databaseService;
        _mainViewModel = mainViewModel;

        QuickAmounts = new ObservableCollection<int>(
            _mainViewModel.Settings.QuickRecordButtons
        );

        RefreshData();
    }

    /// <summary>
    /// 刷新数据
    /// </summary>
    public void RefreshData()
    {
        TodayStats = _databaseService.GetTodayStats();
        QuickAmounts.Clear();
        foreach (var amount in _mainViewModel.Settings.QuickRecordButtons)
        {
            QuickAmounts.Add(amount);
        }
    }

    /// <summary>
    /// 快捷记录饮水
    /// </summary>
    [RelayCommand]
    private void QuickRecord(int amountMl)
    {
        _mainViewModel.AddDrinkRecord(amountMl);
        RefreshData();
    }

    /// <summary>
    /// 打开自定义记录对话框
    /// </summary>
    [RelayCommand]
    private void OpenCustomDialog()
    {
        CustomAmount = 200;
        CustomNote = string.Empty;
        IsCustomDialogOpen = true;
    }

    /// <summary>
    /// 确认自定义记录
    /// </summary>
    [RelayCommand]
    private void ConfirmCustomRecord()
    {
        if (CustomAmount > 0)
        {
            _mainViewModel.AddDrinkRecord(CustomAmount, CustomNote);
            RefreshData();
        }
        IsCustomDialogOpen = false;
    }

    /// <summary>
    /// 取消自定义记录
    /// </summary>
    [RelayCommand]
    private void CancelCustomRecord()
    {
        IsCustomDialogOpen = false;
    }

    /// <summary>
    /// 删除记录
    /// </summary>
    [RelayCommand]
    private void DeleteRecord(long recordId)
    {
        _databaseService.DeleteRecord(recordId);
        RefreshData();
        _mainViewModel.RefreshTodayStats();
    }
}