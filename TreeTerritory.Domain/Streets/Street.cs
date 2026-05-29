using TreeTerritory.Domain.Streets.ValueObjects;

namespace TreeTerritory.Domain.Streets;

public class Street
{
    public required StreetId Id { get; init; }
    public string Name { get; private set; } = string.Empty;

    public ZipCode ZipCode { get; private set; } = ZipCode.Empty;

    public static Street Create(string name, ZipCode zipCode)
    {
        return new Street
        {
            Id = StreetId.From(Guid.NewGuid()),
            Name = name,
            ZipCode = zipCode
        };
    }

    private Street()
    { }

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void UpdateZipCode(ZipCode zipCode)
    {
        ZipCode = zipCode;
    }
}
