using CommunityToolkit.Mvvm.ComponentModel;

namespace ViveGui.Models;

public partial class InstructionRow : ObservableObject
{
    public uint Id { get; set; }
    public uint? Variant { get; set; }
    public ActionType Action { get; set; }

    // 使用 CommunityToolkit 自动生成 OnPropertyChanged 代码
    [ObservableProperty]
    private string _outputText = "Initializing...";

    [ObservableProperty]
    private RowStatus _status = RowStatus.Initializing;

    public string GetIdString
        => Variant.HasValue ? $"{Id} (Var: {Variant})" : $"{Id}";
}
