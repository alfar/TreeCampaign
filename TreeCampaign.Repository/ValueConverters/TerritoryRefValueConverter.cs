using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeCampaign.Domain.ExternalReferences;

namespace TreeCampaign.Repository.ValueConverters;

internal class TerritoryRefValueConverter : ValueConverter<TerritoryRef, Guid>
{
    public TerritoryRefValueConverter()
        : base(territoryRef => territoryRef.Value, value => TerritoryRef.From(value)) { }
}

internal class NullableTerritoryRefValueConverter : ValueConverter<TerritoryRef?, Guid?>
{
    public NullableTerritoryRefValueConverter()
        : base(territoryRef => territoryRef != null ? territoryRef.Value : null, value => value.HasValue ? TerritoryRef.From(value.Value) : null) { }
}
