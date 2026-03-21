using System.Windows;
using System.Windows.Controls;

using Microsoft.Extensions.DependencyInjection;

using ViveGui.Services;
using ViveGui.Services.Backend;
using ViveGui.ViewModels;
using ViveGui.Views;

namespace ViveGui;

public partial class App : Application
{
    public IServiceProvider? Services { get; private set; }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 1. 配置依赖注入
        Services = new ServiceCollection()
            .AddSingleton<IViveToolAdapter, ViveToolAdapter>()
            .AddSingleton<IIdStringParser, IdStringParser>()
            .AddSingleton<MainViewModel>()
            .AddSingleton<MainWindow>()
            .BuildServiceProvider();

        // 2. 环境检测
        if (EnvironmentService.IsVivetoolAvailable is TextBlock errorMsg)
        {
            var uiMessageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = "ViVeTool Missing",
                Content = errorMsg,
                CloseButtonText = "OK",
                CloseButtonAppearance = Wpf.Ui.Controls.ControlAppearance.Primary,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            _ = await uiMessageBox.ShowDialogAsync();

            Shutdown();
            return;
        }

        // 3. 启动主窗口
        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
