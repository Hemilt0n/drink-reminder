using DrinkReminder.Models;
using CommunityToolkit.WinUI.Notifications;

namespace DrinkReminder.Services;

/// <summary>
/// 提醒服务 - 管理定时提醒功能
/// </summary>
public class ReminderService : IDisposable
{
    private Timer? _timer;
    private readonly AppSettings _settings;
    private DateTime _lastReminderTime;
    private bool _disposed;

    /// <summary>
    /// 提醒触发事件
    /// </summary>
    public event EventHandler<ReminderEventArgs>? ReminderTriggered;

    public ReminderService(AppSettings settings)
    {
        _settings = settings;
        _lastReminderTime = DateTime.MinValue;
    }

    /// <summary>
    /// 启动定时提醒
    /// </summary>
    public void Start()
    {
        if (!_settings.ReminderEnabled) return;

        // 每分钟检查一次是否需要提醒
        _timer = new Timer(CheckAndRemind, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
    }

    /// <summary>
    /// 停止提醒
    /// </summary>
    public void Stop()
    {
        _timer?.Dispose();
        _timer = null;
    }

    /// <summary>
    /// 更新设置并重启服务
    /// </summary>
    public void UpdateSettings(AppSettings settings)
    {
        var wasRunning = _timer != null;
        Stop();

        // 更新设置引用
        var settingsType = _settings.GetType();
        foreach (var prop in settingsType.GetProperties())
        {
            if (prop.CanWrite)
            {
                prop.SetValue(_settings, prop.GetValue(settings));
            }
        }

        if (wasRunning || _settings.ReminderEnabled)
        {
            Start();
        }
    }

    /// <summary>
    /// 检查并发送提醒
    /// </summary>
    private void CheckAndRemind(object? state)
    {
        if (!_settings.ReminderEnabled) return;

        var now = DateTime.Now;

        // 检查是否在提醒时间段内
        if (!IsWithinReminderHours(now))
        {
            return;
        }

        // 检查是否到了提醒时间
        var timeSinceLastReminder = now - _lastReminderTime;
        if (timeSinceLastReminder.TotalMinutes >= _settings.ReminderIntervalMinutes)
        {
            SendReminder();
            _lastReminderTime = now;
        }
    }

    /// <summary>
    /// 检查是否在提醒时间段内
    /// </summary>
    private bool IsWithinReminderHours(DateTime time)
    {
        var currentTime = time.TimeOfDay;
        return currentTime >= _settings.ReminderStartTime &&
               currentTime <= _settings.ReminderEndTime;
    }

    /// <summary>
    /// 发送提醒通知
    /// </summary>
    private void SendReminder()
    {
        // 触发事件
        ReminderTriggered?.Invoke(this, new ReminderEventArgs
        {
            Timestamp = DateTime.Now,
            Message = "该喝水啦！保持水分充足对身体很重要哦～"
        });

        // 发送 Windows Toast 通知
        ShowToastNotification();
    }

    /// <summary>
    /// 显示 Windows Toast 通知
    /// </summary>
    private void ShowToastNotification()
    {
        try
        {
            var toast = new ToastContentBuilder()
                .AddText("💧 喝水提醒")
                .AddText("该喝水啦！保持水分充足对身体很重要哦～")
                .AddButton(new ToastButton()
                    .SetContent("记录 200ml")
                    .AddArgument("action", "quickRecord")
                    .AddArgument("amount", "200"))
                .AddButton(new ToastButton()
                    .SetContent("稍后提醒")
                    .AddArgument("action", "snooze"))
                .AddArgument("action", "reminder");

            toast.Show();
        }
        catch (Exception ex)
        {
            // Toast 通知失败时静默处理
            System.Diagnostics.Debug.WriteLine($"Toast notification failed: {ex.Message}");
        }
    }

    /// <summary>
    /// 立即发送一次提醒（用于测试）
    /// </summary>
    public void SendTestReminder()
    {
        SendReminder();
    }

    /// <summary>
    /// 重置提醒计时器（例如在记录饮水后）
    /// </summary>
    public void ResetTimer()
    {
        _lastReminderTime = DateTime.Now;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _timer?.Dispose();
            _disposed = true;
        }
    }
}

/// <summary>
/// 提醒事件参数
/// </summary>
public class ReminderEventArgs : EventArgs
{
    /// <summary>
    /// 提醒时间
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 提醒消息
    /// </summary>
    public string Message { get; set; } = string.Empty;
}