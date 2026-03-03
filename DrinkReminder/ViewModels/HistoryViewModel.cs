using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrinkReminder.Models;
using DrinkReminder.Services;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace DrinkReminder.ViewModels;

/// <summary>
/// 历史统计视图模型
/// </summary>
public partial class HistoryViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private int _selectedDays = 7;

    [ObservableProperty]
    private ObservableCollection<DailyStats> _weeklyStats = new();

    [ObservableProperty]
    private ObservableCollection<ISeries> _chartSeries = new();

    [ObservableProperty]
    private ObservableCollection<Axis> _xAxes = new();

    [ObservableProperty]
    private ObservableCollection<Axis> _yAxes = new();

    [ObservableProperty]
    private DailyStats? _selectedDayStats;

    [ObservableProperty]
    private bool _showDayDetail;

    public HistoryViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;

        // 初始化坐标轴
        YAxes = new ObservableCollection<Axis>
        {
            new Axis
            {
                Name = "饮水量 (ml)",
                LabelsPaint = new SolidColorPaint(SKColors.Gray)
            }
        };

        RefreshData();
    }

    /// <summary>
    /// 刷新数据
    /// </summary>
    public void RefreshData()
    {
        LoadWeeklyStats();
        UpdateChart();
    }

    /// <summary>
    /// 加载周统计
    /// </summary>
    private void LoadWeeklyStats()
    {
        var stats = _databaseService.GetHistory(SelectedDays);
        WeeklyStats.Clear();
        foreach (var stat in stats)
        {
            WeeklyStats.Add(stat);
        }
    }

    /// <summary>
    /// 更新图表
    /// </summary>
    private void UpdateChart()
    {
        var orderedStats = WeeklyStats.OrderBy(s => s.Date).ToList();

        var values = orderedStats
            .Select(s => (double)s.TotalMl)
            .ToArray();

        var goalValues = orderedStats
            .Select(s => (double)s.GoalMl)
            .ToArray();

        ChartSeries.Clear();
        ChartSeries.Add(new ColumnSeries<double>
        {
            Values = values,
            Name = "饮水量",
            Fill = new SolidColorPaint(new SKColor(33, 150, 243))
        });

        ChartSeries.Add(new LineSeries<double>
        {
            Values = goalValues,
            Name = "目标",
            Stroke = new SolidColorPaint(new SKColor(255, 152, 0)) { StrokeThickness = 2 },
            Fill = null,
            GeometrySize = 0
        });

        // 更新 X 轴标签
        var labels = orderedStats
            .Select(s => $"{s.FormattedDate}\n{s.WeekDay}")
            .ToArray();

        XAxes = new ObservableCollection<Axis>
        {
            new Axis
            {
                Labels = labels,
                LabelsPaint = new SolidColorPaint(SKColors.Gray),
                LabelsRotation = 0
            }
        };
    }

    /// <summary>
    /// 选择查看某一天详情
    /// </summary>
    [RelayCommand]
    private void SelectDay(DailyStats? stats)
    {
        if (stats != null)
        {
            SelectedDayStats = stats;
            ShowDayDetail = true;
        }
    }

    /// <summary>
    /// 关闭日期详情
    /// </summary>
    [RelayCommand]
    private void CloseDayDetail()
    {
        ShowDayDetail = false;
        SelectedDayStats = null;
    }

    /// <summary>
    /// 切换到7天视图
    /// </summary>
    [RelayCommand]
    private void Show7Days()
    {
        SelectedDays = 7;
        RefreshData();
    }

    /// <summary>
    /// 切换到14天视图
    /// </summary>
    [RelayCommand]
    private void Show14Days()
    {
        SelectedDays = 14;
        RefreshData();
    }

    /// <summary>
    /// 切换到30天视图
    /// </summary>
    [RelayCommand]
    private void Show30Days()
    {
        SelectedDays = 30;
        RefreshData();
    }
}