namespace DrinkReminder.Models;

/// <summary>
/// 应用设置
/// </summary>
public class AppSettings
{
    /// <summary>
    /// 每日饮水目标(毫升)
    /// </summary>
    public int DailyGoalMl { get; set; } = 2000;

    /// <summary>
    /// 提醒间隔(分钟)
    /// </summary>
    public int ReminderIntervalMinutes { get; set; } = 30;

    /// <summary>
    /// 是否启用定时提醒
    /// </summary>
    public bool ReminderEnabled { get; set; } = true;

    /// <summary>
    /// 提醒开始时间
    /// </summary>
    public TimeSpan ReminderStartTime { get; set; } = new(8, 0, 0);

    /// <summary>
    /// 提醒结束时间
    /// </summary>
    public TimeSpan ReminderEndTime { get; set; } = new(22, 0, 0);

    /// <summary>
    /// 快捷记录按钮配置(毫升)
    /// </summary>
    public List<int> QuickRecordButtons { get; set; } = new() { 200, 300, 500 };

    /// <summary>
    /// 是否开机自启动
    /// </summary>
    public bool AutoStart { get; set; } = false;

    /// <summary>
    /// 是否最小化到托盘
    /// </summary>
    public bool MinimizeToTray { get; set; } = true;

    /// <summary>
    /// 关闭时最小化到托盘而非退出
    /// </summary>
    public bool CloseToTray { get; set; } = true;

    /// <summary>
    /// 主题设置 (light/dark/system)
    /// </summary>
    public string Theme { get; set; } = "system";

    /// <summary>
    /// 格式化的提醒时间段显示
    /// </summary>
    public string FormattedReminderPeriod =>
        $"{ReminderStartTime:hh\\:mm} - {ReminderEndTime:hh\\:mm}";
}