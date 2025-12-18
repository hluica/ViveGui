using Wpf.Ui;
using Wpf.Ui.Controls;
using ViveGui.ViewModels;

namespace ViveGui.Views;

public partial class MainWindow : FluentWindow
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();

        var dialogService = new ContentDialogService();
        dialogService.SetDialogHost(RootContentDialog);
        viewModel.SetDialogService(dialogService);

        DataContext = viewModel;
    }
}