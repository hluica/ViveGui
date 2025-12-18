namespace ViveGui.Models;

public enum ActionType
{
    Query,
    Enable,
    Reset
}

public enum RowStatus
{
    Initializing,
    Skipped,
    Confirming,
    Configured,
    Error
}