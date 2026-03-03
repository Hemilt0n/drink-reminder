namespace DrinkReminder.Models;

/// <summary>
/// 饮水记录
/// </summary>
public class DrinkRecord
{
    /// <summary>
    /// 记录ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 饮水量(毫升)
    /// </summary>
    public int AmountMl { get; set; }

    /// <summary>
    /// 记录时间
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// 格式化的时间显示
    /// </summary>
    public string FormattedTime => Timestamp.ToString("HH:mm");

    /// <summary>
    /// 格式化的日期时间显示
    /// </summary>
    public string FormattedDateTime => Timestamp.ToString("yyyy-MM-dd HH:mm");
}