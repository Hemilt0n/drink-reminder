using DrinkReminder.ViewModels;
using System.Windows.Controls;

namespace DrinkReminder.Views;

/// <summary>
/// HomeView.xaml 的交互逻辑
/// </summary>
public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
    }

    public HomeView(HomeViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}