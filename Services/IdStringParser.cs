namespace ViveGui.Services;

public class IdStringParser
{
    public List<(uint Id, uint? Variant)> ParseIds(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return [];

        return [.. input
            .Split([' ', ',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(ParseIdToken)
            .Where(t => t.HasValue)
            .Select(t => t!.Value)
            .Distinct()];
    }

    private static (uint Id, uint? Variant)? ParseIdToken(string token)
    {
        var parts = token.Split('+');
        if (!uint.TryParse(parts[0], out uint id)) return null;

        uint? variant = null;
        if (parts.Length > 1)
        {
            if (uint.TryParse(parts[1], out uint v)) variant = v;
            else return null;
        }
        return (id, variant);
    }
}