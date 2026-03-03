using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;
using DrinkReminder.Models;

namespace DrinkReminder.Services;

/// <summary>
/// 托盘服务 - 管理系统托盘图标和菜单
/// </summary>
public class TrayService : IDisposable
{
    private TaskbarIcon? _trayIcon;
    private readonly AppSettings _settings;
    private bool _disposed;

    /// <summary>
    /// 显示主窗口请求事件
    /// </summary>
    public event Action? ShowMainWindowRequested;

    /// <summary>
    /// 快速记录饮水事件
    /// </summary>
    public event Action<int>? QuickRecordRequested;

    /// <summary>
    /// 退出应用请求事件
    /// </summary>
    public event Action? ExitRequested;

    public TrayService(AppSettings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// 初始化托盘图标
    /// </summary>
    public void Initialize()
    {
        _trayIcon = new TaskbarIcon
        {
            ToolTipText = "喝水提醒小助手",
            Icon = CreateProgressIcon(0, 100),
            ContextMenu = CreateContextMenu()
        };

        _trayIcon.TrayMouseDoubleClick += (_, _) => ShowMainWindowRequested?.Invoke();
    }

    /// <summary>
    /// 创建托盘右键菜单
    /// </summary>
    private System.Windows.Controls.ContextMenu CreateContextMenu()
    {
        var menu = new System.Windows.Controls.ContextMenu();

        // 进度显示
        var progressItem = new System.Windows.Controls.MenuItem
        {
            Header = "💧 0/2000 ml",
            IsEnabled = false,
            Name = "ProgressItem"
        };
        menu.Items.Add(progressItem);

        menu.Items.Add(new System.Windows.Controls.Separator());

        // 快捷记录按钮
        foreach (var amount in _settings.QuickRecordButtons)
        {
            var recordItem = new System.Windows.Controls.MenuItem
            {
                Header = $"+ 记录 {amount} ml",
                Tag = amount
            };
            recordItem.Click += (_, _) => QuickRecordRequested?.Invoke(amount);
            menu.Items.Add(recordItem);
        }

        // 自定义记录
        var customRecordItem = new System.Windows.Controls.MenuItem
        {
            Header = "+ 自定义记录..."
        };
        customRecordItem.Click += (_, _) => ShowMainWindowRequested?.Invoke();
        menu.Items.Add(customRecordItem);

        menu.Items.Add(new System.Windows.Controls.Separator());

        // 显示主窗口
        var showWindowItem = new System.Windows.Controls.MenuItem
        {
            Header = "显示主窗口"
        };
        showWindowItem.Click += (_, _) => ShowMainWindowRequested?.Invoke();
        menu.Items.Add(showWindowItem);

        // 设置
        var settingsItem = new System.Windows.Controls.MenuItem
        {
            Header = "设置"
        };
        settingsItem.Click += (_, _) =>
        {
            ShowMainWindowRequested?.Invoke();
            // TODO: 导航到设置页
        };
        menu.Items.Add(settingsItem);

        menu.Items.Add(new System.Windows.Controls.Separator());

        // 退出
        var exitItem = new System.Windows.Controls.MenuItem
        {
            Header = "退出"
        };
        exitItem.Click += (_, _) => ExitRequested?.Invoke();
        menu.Items.Add(exitItem);

        return menu;
    }

    /// <summary>
    /// 更新托盘图标显示的进度
    /// </summary>
    public void UpdateProgress(int currentMl, int goalMl)
    {
        if (_trayIcon == null) return;

        var progressPercent = goalMl > 0 ? (int)((double)currentMl / goalMl * 100) : 0;
        progressPercent = Math.Min(progressPercent, 100);

        // 更新图标
        _trayIcon.Icon = CreateProgressIcon(progressPercent, goalMl);

        // 更新工具提示文本
        _trayIcon.ToolTipText = $"喝水提醒 - {currentMl}/{goalMl} ml ({progressPercent}%)";

        // 更新菜单中的进度显示
        if (_trayIcon.ContextMenu?.Items[0] is System.Windows.Controls.MenuItem progressItem)
        {
            progressItem.Header = $"💧 {currentMl}/{goalMl} ml";
        }
    }

    /// <summary>
    /// 创建进度图标
    /// </summary>
    private Icon CreateProgressIcon(int progressPercent, int goalMl)
    {
        const int size = 32;
        using var bitmap = new Bitmap(size, size);
        using var g = Graphics.FromImage(bitmap);

        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.Clear(Color.Transparent);

        // 根据进度选择颜色
        Color progressColor = progressPercent switch
        {
            >= 100 => Color.FromArgb(76, 175, 80),  // 绿色 - 已完成
            >= 70 => Color.FromArgb(255, 193, 7),   // 黄色 - 接近目标
            >= 50 => Color.FromArgb(255, 152, 0),   // 橙色 - 中等进度
            _ => Color.FromArgb(33, 150, 243)       // 蓝色 - 刚开始
        };

        // 绘制背景圆环
        using var bgPen = new Pen(Color.FromArgb(60, Color.White), 4);
        g.DrawEllipse(bgPen, 4, 4, size - 8, size - 8);

        // 绘制进度圆弧
        if (progressPercent > 0)
        {
            using var progressPen = new Pen(progressColor, 4);
            var angle = (float)(progressPercent * 3.6);
            g.DrawArc(progressPen, 4, 4, size - 8, size - 8, -90, angle);
        }

        // 绘制中心水滴图标
        DrawWaterDrop(g, size / 2, size / 2, 8, progressColor);

        // 转换为图标
        var hIcon = bitmap.GetHicon();
        return Icon.FromHandle(hIcon);
    }

    /// <summary>
    /// 绘制水滴图标
    /// </summary>
    private void DrawWaterDrop(Graphics g, int centerX, int centerY, int size, Color color)
    {
        using var brush = new SolidBrush(color);

        // 水滴形状：使用多边形近似
        var points = new PointF[7];
        for (int i = 0; i < 7; i++)
        {
            double angle = Math.PI * 2 * i / 6 - Math.PI / 2;
            double r = size * (i % 2 == 0 ? 1 : 0.6);

            // 调整底部使其更圆润
            if (i >= 2 && i <= 4)
            {
                r = size * 0.9;
            }

            points[i] = new PointF(
                centerX + (float)(r * Math.Cos(angle)),
                centerY + (float)(r * Math.Sin(angle))
            );
        }

        g.FillPolygon(brush, points);
    }

    /// <summary>
    /// 显示托盘气泡通知
    /// </summary>
    public void ShowBalloonTip(string title, string message, BalloonIcon icon = BalloonIcon.Info)
    {
        _trayIcon?.ShowBalloonTip(title, message, icon);
    }

    /// <summary>
    /// 更新快捷记录菜单
    /// </summary>
    public void UpdateQuickRecordMenu(List<int> amounts)
    {
        if (_trayIcon?.ContextMenu == null) return;

        // 找到并移除旧的快捷记录项
        var itemsToRemove = _trayIcon.ContextMenu.Items
            .Cast<System.Windows.Controls.MenuItem>()
            .Where(item => item.Tag is int)
            .ToList();

        foreach (var item in itemsToRemove)
        {
            _trayIcon.ContextMenu.Items.Remove(item);
        }

        // 在分隔符后插入新的快捷记录项
        var insertIndex = 2; // 进度项 + 分隔符之后

        foreach (var amount in amounts)
        {
            var recordItem = new System.Windows.Controls.MenuItem
            {
                Header = $"+ 记录 {amount} ml",
                Tag = amount
            };
            recordItem.Click += (_, _) => QuickRecordRequested?.Invoke(amount);
            _trayIcon.ContextMenu.Items.Insert(insertIndex++, recordItem);
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _trayIcon?.Dispose();
            _disposed = true;
        }
    }
}