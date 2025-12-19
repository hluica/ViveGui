using ViveGui.ViewModels;

using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace ViveGui.Views;

public partial class MainWindow : FluentWindow
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();

        SystemThemeWatcher.Watch(this);

        var dialogService = new ContentDialogService();
        dialogService.SetDialogHost(RootContentDialog);
        viewModel.SetDialogService(dialogService);

        DataContext = viewModel;
    }
}
