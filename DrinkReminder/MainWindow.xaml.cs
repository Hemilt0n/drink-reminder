using DrinkReminder.ViewModels;
using System.Windows;

namespace DrinkReminder;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel? _viewModel;
    private bool _isReallyClosing;

    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(MainViewModel viewModel) : this()
    {
        _viewModel = viewModel;
        DataContext = viewModel;
    }

    private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        var settings = _viewModel?.Settings;
        if (settings?.CloseToTray == true && !_isReallyClosing)
        {
            e.Cancel = true;
            Hide();
        }
    }

    public void ReallyClose()
    {
        _isReallyClosing = true;
        Close();
    }
}