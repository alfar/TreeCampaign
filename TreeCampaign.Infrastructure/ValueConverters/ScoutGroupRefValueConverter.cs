using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeCampaign.Domain.ExternalReferences;

namespace TreeCampaign.Infrastructure.ValueConverters;

internal class ScoutGroupRefValueConverter : ValueConverter<ScoutGroupRef, Guid>
{
    public ScoutGroupRefValueConverter()
        : base(scoutGroupRef => scoutGroupRef.Value, value => ScoutGroupRef.From(value)) { }
}
