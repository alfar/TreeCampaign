using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Infrastructure.ValueConverters;

internal class HouseNumberValueConverter : ValueConverter<HouseNumber, string>
{
    public HouseNumberValueConverter()
        : base(number => $"{number.Number}{number.Letter}", value => HouseNumber.Parse(value)) { }
}

internal class NullableHouseNumberValueConverter : ValueConverter<HouseNumber?, string?>
{
    public NullableHouseNumberValueConverter()
        : base(number => number == null ? null : number.ToString(), value => value == null ? null : HouseNumber.Parse(value)) { }
}