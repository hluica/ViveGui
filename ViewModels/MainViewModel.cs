using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ViveGui.Models;
using ViveGui.Services;
using ViveGui.Services.Backend;

using Wpf.Ui;
using Wpf.Ui.Controls;

namespace ViveGui.ViewModels;

public partial class MainViewModel(IViveToolAdapter adapter, IIdStringParser parser) : ObservableObject
{
    private readonly IViveToolAdapter _adapter = adapter;
    private readonly IIdStringParser _parser = parser;
    private IContentDialogService? _contentDialogService;

    [ObservableProperty]
    private ObservableCollection<InstructionRow> _instructions = [];

    [ObservableProperty]
    private string _inputText = string.Empty;

    // 绑定到下拉菜单的选定项
    [ObservableProperty]
    private ActionType _selectedAction = ActionType.Query;

    // 提供给下拉菜单的数据源
    public static IEnumerable<ActionType> ActionTypes
        => Enum.GetValues<ActionType>();

    public void SetDialogService(IContentDialogService service)
        => _contentDialogService = service;

    // 界面锁定状态
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    private bool _isBusy = false;

    [RelayCommand(CanExecute = nameof(CanInteract))]
    private void Add()
    {
        var ids = _parser.ParseIds(InputText);
        if (ids.Count == 0)
            return;

        foreach (var (id, variant) in ids)
        {
            // 简单去重逻辑：移除已存在的相同 ID 行
            var existing = Instructions.FirstOrDefault(x => x.Id == id && x.Variant == variant);
            if (existing != null)
                _ = Instructions.Remove(existing);

            Instructions.Add(new InstructionRow
            {
                Id = id,
                Variant = variant,
                Action = SelectedAction,
                Status = SelectedAction == ActionType.Query ? RowStatus.Skipped : RowStatus.Confirming,
                OutputText = "Ready to execute"
            });
        }

        // 自动执行查询动作
        _ = RunQueryAsync([.. ids.Select(x => x.Id)]);

        InputText = string.Empty; // 清空输入框
    }

    [RelayCommand(CanExecute = nameof(CanInteract))]
    private async Task Run()
    {
        IsBusy = true;
        var executableRows = Instructions
            .Where(r => r.Action is ActionType.Enable or ActionType.Reset)
            .ToList();

        if (executableRows.Count > 0)
            await _adapter.ExecuteBatchAsync(executableRows);
        IsBusy = false;

        if (_contentDialogService != null)
        {
            var dialog = new ContentDialog()
            {
                Title = "Done",
                Content = "Batch execution finished.",
                CloseButtonText = "OK",
                CloseButtonAppearance = ControlAppearance.Primary
            };
            _ = await _contentDialogService.ShowAsync(dialog, CancellationToken.None);
        }
    }

    private async Task RunQueryAsync(List<uint> ids)
    {
        // 简单的单个查询逻辑
        foreach (var row in Instructions.Where(x => ids.Contains(x.Id)))
        {
            row.OutputText = "Querying...";
            row.OutputText = await _adapter.QuerySingleAsync(row.Id);
        }
    }

    [RelayCommand(CanExecute = nameof(CanInteract))]
    private void Clear()
        => Instructions.Clear();

    [RelayCommand]
    private void RemoveRow(InstructionRow row)
        => Instructions.Remove(row);

    private bool CanInteract()
        => !IsBusy;
}
