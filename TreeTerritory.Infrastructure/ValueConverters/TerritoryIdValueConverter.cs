using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeTerritory.Domain.Territories.ValueObjects;

namespace TreeTerritory.Infrastructure.ValueConverters;

internal class TerritoryIdValueConverter : ValueConverter<TerritoryId, Guid>
{
    public TerritoryIdValueConverter()
        : base(id => id.Value, value => TerritoryId.From(value)) { }
}
