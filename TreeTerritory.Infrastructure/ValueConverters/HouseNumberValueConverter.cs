using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeTerritory.Domain.Streets.ValueObjects;
using TreeTerritory.Domain.StreetSections.ValueObjects;

namespace TreeTerritory.Infrastructure.ValueConverters;

internal class HouseNumberValueConverter : ValueConverter<HouseNumber, string>
{
    public HouseNumberValueConverter()
        : base(number => $"{number.Number}{number.Letter}", value => HouseNumber.Parse(value)) { }
}

internal class NullableHouseNumberValueConverter : ValueConverter<HouseNumber?, string?>
{
    public NullableHouseNumberValueConverter()
        : base(number => number == null ? null : $"{number.Number}{number.Letter}", value => value == null ? null : HouseNumber.Parse(value)) { }
}
