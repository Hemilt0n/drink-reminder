using DrinkReminder.ViewModels;
using Wpf.Ui.Controls;

namespace DrinkReminder;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : FluentWindow
{
    private MainViewModel? ViewModel => DataContext as MainViewModel;
    private bool _isReallyClosing;

    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(MainViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        var settings = ViewModel?.Settings;
        if (settings?.CloseToTray == true && !_isReallyClosing)
        {
            e.Cancel = true;
            Hide();
        }
    }

    private void OnWindowStateChanged(object sender, EventArgs e)
    {
        var settings = ViewModel?.Settings;
        if (settings?.MinimizeToTray == true && WindowState == System.Windows.WindowState.Minimized)
        {
            Hide();
        }
    }

    public void ReallyClose()
    {
        _isReallyClosing = true;
        Close();
    }
}
