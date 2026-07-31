using Intake.Domain.ExternalReferences;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Intake.Infrastructure.ValueConverters;

internal class TerritoryRefValueConverter : ValueConverter<TerritoryRef, Guid>
{
    public TerritoryRefValueConverter() : base(id => id.Value, value => new TerritoryRef(value)) { }
}

internal class NullableTerritoryRefValueConverter : ValueConverter<TerritoryRef?, Guid?>
{
    public NullableTerritoryRefValueConverter() : base(id => id == null ? null : id.Value, value => value == null ? null : new TerritoryRef(value.Value)) { }
}
