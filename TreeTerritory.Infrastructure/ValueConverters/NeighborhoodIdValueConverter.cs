using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeTerritory.Domain.Neighborhoods.ValueObjects;

namespace TreeTerritory.Infrastructure.ValueConverters;

internal class NeighborhoodIdValueConverter : ValueConverter<NeighborhoodId, Guid>
{
    public NeighborhoodIdValueConverter()
        : base(id => id.Value, value => NeighborhoodId.From(value)) { }
}
