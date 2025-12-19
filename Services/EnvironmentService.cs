using System.IO;

namespace ViveGui.Services;

public class EnvironmentService
{
    public static bool IsVivetoolAvailable(out string errorMessage)
    {
        errorMessage = string.Empty;
        var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        var paths = pathEnv.Split(Path.PathSeparator);

        foreach (var path in paths)
        {
            var fullPath = Path.Combine(path, "vivetool.exe");
            if (File.Exists(fullPath)) return true;
        }

        errorMessage = "Error: 'vivetool.exe' was not found in your PATH.\nPlease download it and add it to your system environment variables.";
        return false;
    }
}
