using Intake.Domain.ExternalReferences;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Intake.InfraStructure.ValueConverters;

internal class StreetRefValueConverter : ValueConverter<StreetRef, Guid>
{
    public StreetRefValueConverter() : base(r => r.Value, value => StreetRef.From(value)) { }
}
