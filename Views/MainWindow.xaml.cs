using ViveGui.ViewModels;

using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace ViveGui.Views;

public partial class MainWindow : FluentWindow
{
    public MainWindow(MainViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();

        SystemThemeWatcher.Watch(this);
    }
}
