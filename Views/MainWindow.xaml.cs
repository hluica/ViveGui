using System.Windows;
using System.Windows.Media;

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

    private async void OnCopyContentButtonClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Content is SymbolIcon icon)
        {
            if (btn.Tag is not string textToCopy || string.IsNullOrWhiteSpace(textToCopy))
            {
                return;
            }

            // 1. 记录按钮的原始状态，以便稍后恢复
            var originalSymbol = icon.Symbol;
            var originalForeground = icon.Foreground;

            // 暂时禁用按钮，防止用户疯狂连击
            btn.IsEnabled = false;

            try
            {
                // 2. 尝试复制内容到剪贴板（包含重试机制）
                bool isSuccess = await TryCopyToClipboardAsync(textToCopy);

                // 3. 根据结果提供视觉反馈
                if (isSuccess)
                {
                    icon.Symbol = SymbolRegular.Checkmark24;
                    icon.Foreground = Brushes.MediumSeaGreen;
                }
                else
                {
                    icon.Symbol = SymbolRegular.Warning24;
                    icon.Foreground = Brushes.IndianRed;
                }

                // 4. 保持反馈状态 2 秒钟
                await Task.Delay(2000);
            }
            finally
            {
                // 5. 恢复原始状态
                icon.Symbol = originalSymbol;
                icon.Foreground = originalForeground;
                btn.IsEnabled = true;
            }
        }
    }

    private static async Task<bool> TryCopyToClipboardAsync(string text)
    {
        // WPF 的 Clipboard 操作极易因为其他程序正在读
        // 取剪贴板而抛出 CLIPBRD_E_CANT_OPEN 异常。
        // 标准的解决方案是循环重试几次。
        for (int i = 0; i < 5; i++)
        {
            try
            {
                Clipboard.SetText(text);
                return true;
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                await Task.Delay(20);
            }
            catch (Exception)
            {
                break;
            }
        }
        return false;
    }
}
