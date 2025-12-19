using System.Windows;

using Microsoft.Extensions.DependencyInjection;

using ViveGui.Services;
using ViveGui.Services.Backend;
using ViveGui.ViewModels;
using ViveGui.Views;

namespace ViveGui;

public partial class App : Application
{
    public new static App Current => (App)Application.Current;
    public IServiceProvider? Services { get; private set; }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 1. 配置依赖注入
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        Services = serviceCollection.BuildServiceProvider();

        // 2. 环境检测
        _ = Services.GetRequiredService<EnvironmentService>();
        if (!EnvironmentService.IsVivetoolAvailable(out string errorMsg))
        {
            var uiMessageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = "ViVeTool Missing",
                Content = errorMsg,
                CloseButtonText = "OK",
                // 设置窗口居中显示，因为此时没有主窗口作为 owner
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            await uiMessageBox.ShowDialogAsync();

            Shutdown();
            return;
        }

        // 3. 启动主窗口
        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Models & Services
        services.AddSingleton<IViveToolAdapter, ViveToolAdapter>();
        services.AddSingleton<EnvironmentService>();
        services.AddSingleton<IdStringParser>();

        // ViewModels
        services.AddSingleton<MainViewModel>();

        // Views
        services.AddSingleton<MainWindow>();
    }
}
