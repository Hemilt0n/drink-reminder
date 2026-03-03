using DrinkReminder.Services;
using DrinkReminder.ViewModels;
using System.Windows;
using System.Windows.Threading;

namespace DrinkReminder;

/// <summary>
/// App.xaml 的交互逻辑
/// </summary>
public partial class App : Application
{
    private DatabaseService? _databaseService;
    private TrayService? _trayService;
    private ReminderService? _reminderService;
    private MainViewModel? _mainViewModel;
    private MainWindow? _mainWindow;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 初始化服务
        _databaseService = new DatabaseService();
        var settings = _databaseService.GetSettings();

        _trayService = new TrayService(settings);
        _reminderService = new ReminderService(settings);

        // 初始化 ViewModel
        _mainViewModel = new MainViewModel(_databaseService, _trayService, _reminderService);

        // 订阅事件
        _mainViewModel.ShowMainWindowRequested += ShowMainWindow;
        _mainViewModel.ExitRequested += ExitApplication;

        // 初始化托盘
        _trayService.Initialize();

        // 启动提醒服务
        _reminderService.Start();

        // 创建主窗口
        _mainWindow = new MainWindow
        {
            DataContext = _mainViewModel
        };

        // 根据设置决定是否显示主窗口
        if (!e.Args.Contains("--minimized"))
        {
            _mainWindow.Show();
        }
    }

    private void ShowMainWindow()
    {
        if (_mainWindow != null)
        {
            _mainWindow.Show();
            _mainWindow.Activate();
            _mainWindow.WindowState = WindowState.Normal;
        }
    }

    private void ExitApplication()
    {
        _mainWindow?.Close();
        Current.Shutdown();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _databaseService?.Dispose();
        _trayService?.Dispose();
        _reminderService?.Dispose();
        base.OnExit(e);
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(
            $"发生未处理的异常:\n{e.Exception.Message}\n\n{e.Exception.StackTrace}",
            "错误",
            MessageBoxButton.OK,
            MessageBoxImage.Error
        );
        e.Handled = true;
    }
}
