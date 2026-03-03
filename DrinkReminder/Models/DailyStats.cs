namespace DrinkReminder.Models;

/// <summary>
/// 每日饮水统计
/// </summary>
public class DailyStats
{
    /// <summary>
    /// 日期
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// 总饮水量(毫升)
    /// </summary>
    public int TotalMl { get; set; }

    /// <summary>
    /// 目标饮水量(毫升)
    /// </summary>
    public int GoalMl { get; set; }

    /// <summary>
    /// 完成进度(0-1)
    /// </summary>
    public double Progress => GoalMl > 0 ? Math.Min((double)TotalMl / GoalMl, 1.0) : 0;

    /// <summary>
    /// 完成百分比
    /// </summary>
    public int ProgressPercent => (int)Math.Round(Progress * 100);

    /// <summary>
    /// 是否完成目标
    /// </summary>
    public bool IsGoalMet => TotalMl >= GoalMl;

    /// <summary>
    /// 饮水记录列表
    /// </summary>
    public List<DrinkRecord> Records { get; set; } = new();

    /// <summary>
    /// 记录数量
    /// </summary>
    public int RecordCount => Records.Count;

    /// <summary>
    /// 格式化的日期显示
    /// </summary>
    public string FormattedDate => Date.ToString("MM月dd日");

    /// <summary>
    /// 星期几显示
    /// </summary>
    public string WeekDay => Date.DayOfWeek switch
    {
        System.DayOfWeek.Monday => "周一",
        System.DayOfWeek.Tuesday => "周二",
        System.DayOfWeek.Wednesday => "周三",
        System.DayOfWeek.Thursday => "周四",
        System.DayOfWeek.Friday => "周五",
        System.DayOfWeek.Saturday => "周六",
        System.DayOfWeek.Sunday => "周日",
        _ => ""
    };
}