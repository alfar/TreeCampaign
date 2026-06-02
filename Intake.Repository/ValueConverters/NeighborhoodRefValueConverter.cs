using Intake.Domain.ExternalReferences;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Intake.Repository.ValueConverters;

internal class NeighborhoodRefValueConverter : ValueConverter<NeighborhoodRef, Guid>
{
    public NeighborhoodRefValueConverter() : base(r => r.Value, value => NeighborhoodRef.From(value)) { }
}
