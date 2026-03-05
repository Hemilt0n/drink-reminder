using DrinkReminder.Helpers;
using DrinkReminder.Services;
using DrinkReminder.ViewModels;
using System.Windows;
using System.Windows.Threading;

namespace DrinkReminder;

/// <summary>
/// App.xaml interaction logic.
/// </summary>
public partial class App : Application
{
    private DatabaseService? _databaseService;
    private TrayService? _trayService;
    private ReminderService? _reminderService;
    private MainViewModel? _mainViewModel;
    private MainWindow? _mainWindow;

    private string? _lastUnhandledExceptionKey;
    private DateTime _lastUnhandledExceptionAtUtc = DateTime.MinValue;
    private bool _isHandlingUnhandledException;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _databaseService = new DatabaseService();
        var settings = _databaseService.GetSettings();

        _trayService = new TrayService(settings);
        _reminderService = new ReminderService(settings);
        ThemeHelper.ApplyTheme(settings.Theme);

        _mainViewModel = new MainViewModel(_databaseService, _trayService, _reminderService);

        _mainViewModel.ShowMainWindowRequested += ShowMainWindow;
        _mainViewModel.ExitRequested += ExitApplication;

        _trayService.Initialize();
        _reminderService.Start();

        _mainWindow = new MainWindow(_mainViewModel);

        if (!e.Args.Contains("--minimized"))
        {
            _mainWindow.Show();
        }
    }

    private void ShowMainWindow()
    {
        if (_mainWindow == null)
        {
            return;
        }

        _mainWindow.Show();
        _mainWindow.Activate();
        _mainWindow.WindowState = WindowState.Normal;
    }

    private void ExitApplication()
    {
        _mainWindow?.ReallyClose();
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
        e.Handled = true;

        if (_isHandlingUnhandledException)
        {
            return;
        }

        var currentKey = $"{e.Exception.GetType().FullName}:{e.Exception.Message}";
        var nowUtc = DateTime.UtcNow;

        if (string.Equals(_lastUnhandledExceptionKey, currentKey, StringComparison.Ordinal)
            && nowUtc - _lastUnhandledExceptionAtUtc < TimeSpan.FromSeconds(3))
        {
            return;
        }

        _isHandlingUnhandledException = true;
        _lastUnhandledExceptionKey = currentKey;
        _lastUnhandledExceptionAtUtc = nowUtc;

        try
        {
            MessageBox.Show(
                $"发生未处理的异常:\n{e.Exception.Message}",
                "应用错误",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            _isHandlingUnhandledException = false;
        }
    }
}

