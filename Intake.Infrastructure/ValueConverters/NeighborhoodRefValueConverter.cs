using Intake.Domain.ExternalReferences;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Intake.InfraStructure.ValueConverters;

internal class NeighborhoodRefValueConverter : ValueConverter<NeighborhoodRef, Guid>
{
    public NeighborhoodRefValueConverter() : base(r => r.Value, value => NeighborhoodRef.From(value)) { }
}

internal class NullableNeighborhoodRefValueConverter : ValueConverter<NeighborhoodRef?, Guid?>
{
    public NullableNeighborhoodRefValueConverter() : base(r => r != null ? r.Value : null, value => value.HasValue ? NeighborhoodRef.From(value.Value) : null) { }
}
