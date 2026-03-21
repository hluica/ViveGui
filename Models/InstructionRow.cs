using CommunityToolkit.Mvvm.ComponentModel;

namespace ViveGui.Models;

public partial class InstructionRow : ObservableObject
{
    public uint Id { get; set; }
    public uint? Variant { get; set; }
    public ActionType Action { get; set; }

    [ObservableProperty]
    public partial string OutputText { get; set; } = "Initializing...";

    [ObservableProperty]
    public partial RowStatus Status { get; set; } = RowStatus.Initializing;

    public string GetIdString
        => Variant.HasValue ? $"{Id} (Var: {Variant})" : $"{Id}";
}
