using System.IO;

namespace ViveGui.Services;

public static class EnvironmentService
{
    public static (bool Success, string ErrorMsg) IsVivetoolAvailable { get; }
        = CheckVivetoolAvailability();

    private static (bool Success, string ErrorMsg) CheckVivetoolAvailability()
    {
        string pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        string[] paths = pathEnv.Split(Path.PathSeparator);

        foreach (string path in paths)
        {
            string fullPath = Path.Combine(path, "vivetool.exe");
            if (File.Exists(fullPath))
                return (true, string.Empty);
        }

        string errorMessage = "Error: The program will not start since 'vivetool.exe' was not found in your PATH.\nPlease download it and add it to your PATH environment variable, then try again.";
        return (false, errorMessage);
    }
}
