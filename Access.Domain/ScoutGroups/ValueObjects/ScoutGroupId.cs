namespace Access.Domain.ScoutGroups.ValueObjects;

public record ScoutGroupId(Guid Value)
{
    public static bool TryParse(string? input, out ScoutGroupId scoutGroupId)
    {
        if (Guid.TryParse(input, out var guid))
        {
            scoutGroupId = From(guid);
            return true;
        }

        scoutGroupId = From(Guid.Empty);
        return false;
    }

    public static ScoutGroupId From(Guid value) => new ScoutGroupId(value);
}
