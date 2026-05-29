namespace TreeTerritory.Domain.Territories.ValueObjects;

public record GroupId(Guid Value)
{
    public static bool TryParse(string? input, out GroupId groupId)
    {
        if (Guid.TryParse(input, out var guid))
        {
            groupId = From(guid);
            return true;
        }

        groupId = From(Guid.Empty);
        return false;
    }

    public static GroupId From(Guid value) => new GroupId(value);
}