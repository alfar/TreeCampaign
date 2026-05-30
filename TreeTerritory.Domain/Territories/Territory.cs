using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.Territories.ValueObjects;

namespace TreeTerritory.Domain.Territories;

public class Territory
{
    public required TerritoryId Id { get; init; }
    public required string Name { get; init; }
    public ZipCode? DefaultZipCode { get; private set; }

    public static Territory Create(string name, ZipCode? defaultZipCode = null)
    {
        return new Territory
        {
            Id = TerritoryId.From(Guid.NewGuid()),
            Name = name,
            DefaultZipCode = defaultZipCode
        };
    }

    private Territory() { }
}