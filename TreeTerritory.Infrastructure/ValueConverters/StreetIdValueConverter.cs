using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeTerritory.Domain.Streets.ValueObjects;

namespace TreeTerritory.InfraStructure.ValueConverters;

internal class StreetIdValueConverter : ValueConverter<StreetId, Guid>
{
    public StreetIdValueConverter()
        : base(id => id.Value, value => StreetId.From(value)) { }
}
