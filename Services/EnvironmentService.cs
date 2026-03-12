using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ViveGui.Services;

public static class EnvironmentService
{
    public static TextBlock? IsVivetoolAvailable { get; }
        = CheckVivetoolAvailability();

    private static TextBlock? CheckVivetoolAvailability()
    {
        string pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        string[] paths = pathEnv.Split(Path.PathSeparator);

        foreach (string path in paths)
        {
            string fullPath = Path.Combine(path, "vivetool.exe");
            if (File.Exists(fullPath))
                return null;
        }

        return CreateErrorTextBlock();
    }

    private static TextBlock CreateErrorTextBlock()
        => new TextBlock { TextWrapping = TextWrapping.Wrap }
            .AddBold("Error:")
            .AddRun(" The program will not start since 'vivetool.exe' was not found in your PATH.")
            .AddLineBreak()
            .AddRun("Please download it from the ")
            .AddHyperlink("official repository", "https://github.com/thebookisclosed/ViVe/releases")
            .AddRun(" and add it to your PATH environment variable, then try again.");
}

internal static class TextBlockExtensions
{
    public static TextBlock AddRun(this TextBlock tb, string text)
    {
        tb.Inlines.Add(new Run(text));
        return tb;
    }

    public static TextBlock AddBold(this TextBlock tb, string text)
    {
        tb.Inlines.Add(new Bold(new Run(text)));
        return tb;
    }

    public static TextBlock AddLineBreak(this TextBlock tb)
    {
        tb.Inlines.Add(new LineBreak());
        return tb;
    }

    public static TextBlock AddHyperlink(this TextBlock tb, string text, string uri)
    {
        var link = new Hyperlink(new Run(text)) { NavigateUri = new Uri(uri) };
        link.RequestNavigate += (_, e) =>
        {
            _ = Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        };
        tb.Inlines.Add(link);
        return tb;
    }
}
