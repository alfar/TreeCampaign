using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeTerritory.Domain.StreetSections.ValueObjects;

namespace TreeTerritory.InfraStructure.ValueConverters;

internal class StreetSectionIdValueConverter : ValueConverter<StreetSectionId, Guid>
{
    public StreetSectionIdValueConverter()
        : base(id => id.Value, value => StreetSectionId.From(value)) { }
}
