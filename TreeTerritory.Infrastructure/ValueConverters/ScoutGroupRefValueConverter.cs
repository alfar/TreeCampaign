using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeTerritory.Domain.ExternalReferences;

namespace TreeTerritory.Infrastructure.ValueConverters;

internal class ScoutGroupRefValueConverter : ValueConverter<ScoutGroupRef, Guid>
{
    public ScoutGroupRefValueConverter()
        : base(scoutGroupRef => scoutGroupRef.Value, value => ScoutGroupRef.From(value)) { }
}
