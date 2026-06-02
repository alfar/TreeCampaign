using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.StreetSections.ValueObjects;

namespace TreeTerritory.InfraStructure.ValueConverters;

internal class HouseNumberValueConverter : ValueConverter<HouseNumber, string>
{
    public HouseNumberValueConverter()
        : base(number => $"{number.Number}{number.Letter}", value => HouseNumber.Parse(value)) { }
}
