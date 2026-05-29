using TreeTerritory.Domain.Territories.ValueObjects;

namespace TreeTerritory.Domain.Territories;

public class Territory
{
    public required TerritoryId Id { get; init; }
    public required string Name { get; init; }

    public static Territory Create(string name)
    {
        return new Territory
        {
            Id = TerritoryId.From(Guid.NewGuid()),
            Name = name
        };
    }

    private Territory() { }
}