using CommunityToolkit.Mvvm.ComponentModel;

namespace ViveGui.Models;

public partial class InstructionRow : ObservableObject
{
    public uint Id { get; set; }
    public uint? Variant { get; set; }
    public ActionType Action { get; set; }

    [ObservableProperty]
    private string _outputText = "Initializing...";

    [ObservableProperty]
    private RowStatus _status = RowStatus.Initializing;

    public string GetIdString
        => Variant.HasValue ? $"{Id} (Var: {Variant})" : $"{Id}";
}
