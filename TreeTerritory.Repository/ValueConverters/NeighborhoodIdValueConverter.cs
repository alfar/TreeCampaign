using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeTerritory.Domain.Neighborhoods.ValueObjects;

namespace TreeTerritory.Repository.ValueConverters;

internal class NeighborhoodIdValueConverter : ValueConverter<NeighborhoodId, Guid>
{
    public NeighborhoodIdValueConverter()
        : base(id => id.Value, value => NeighborhoodId.From(value)) { }
}
