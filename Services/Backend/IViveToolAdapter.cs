using ViveGui.Models;

namespace ViveGui.Services.Backend;

public interface IViveToolAdapter
{
    Task<string> QuerySingleAsync(uint id);
    Task ExecuteBatchAsync(List<InstructionRow> rows);
}
