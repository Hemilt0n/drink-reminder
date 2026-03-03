using DrinkReminder.ViewModels;
using System.Windows.Controls;

namespace DrinkReminder.Views;

/// <summary>
/// HistoryView.xaml 的交互逻辑
/// </summary>
public partial class HistoryView : UserControl
{
    public HistoryView()
    {
        InitializeComponent();
    }

    public HistoryView(HistoryViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}