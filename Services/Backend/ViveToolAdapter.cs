using System.Diagnostics;
using System.Text;

using ViveGui.Models;

namespace ViveGui.Services.Backend;

public class ViveToolAdapter : IViveToolAdapter
{
    public async Task<string> QuerySingleAsync(uint id)
        => await RunProcessAsync($"/query /id:{id}");

    public async Task ExecuteBatchAsync(List<InstructionRow> rows)
    {
        static async Task RunAndUpdateAsync(string args, IEnumerable<InstructionRow> targetRows)
        {
            string output = await RunProcessAsync(args);
            bool isError = output.Contains("Failed", StringComparison.OrdinalIgnoreCase)
                || output.Contains("Error", StringComparison.OrdinalIgnoreCase);
            var newStatus = isError ? RowStatus.Error : RowStatus.Configured;

            foreach (var row in targetRows)
            {
                row.OutputText = output;
                row.Status = newStatus;
            }
        }

        var resetTasks = rows
            .Where(r => r.Action == ActionType.Reset)
            .GroupBy(_ => 0)
            .Select(g => (Args: $"/reset /id:{string.Join(",", g.Select(r => r.Id))}", Rows: g as IEnumerable<InstructionRow>));

        var enableTasks = rows
            .Where(r => r.Action == ActionType.Enable)
            .GroupBy(r => r.Variant)
            .Select(g => (Args: $"/enable /id:{string.Join(",", g.Select(r => r.Id))}{(g.Key.HasValue ? $" /variant:{g.Key}" : "")}", Rows: g as IEnumerable<InstructionRow>));

        var allTasks = resetTasks.Concat(enableTasks);

        foreach (var (Args, Rows) in allTasks)
            await RunAndUpdateAsync(Args, Rows);
    }

    private static async Task<string> RunProcessAsync(string arguments)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "vivetool.exe",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using var process = new Process { StartInfo = psi };
            var outputBuilder = new StringBuilder();
            process.OutputDataReceived += (s, e) =>
            {
                if (e.Data != null)
                    _ = outputBuilder.AppendLine(e.Data);
            };
            process.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null)
                    _ = outputBuilder.AppendLine(e.Data);
            };

            _ = process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync();

            string[] lines = outputBuilder.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            return lines.Length >= 2
                ? string.Join(Environment.NewLine, lines.Skip(1)).Trim()
                : outputBuilder.ToString().Trim();
        }
        catch (Exception ex)
        {
            return $"EXECUTION ERROR: {ex.Message}";
        }
    }
}
