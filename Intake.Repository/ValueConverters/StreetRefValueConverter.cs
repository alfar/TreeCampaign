using Intake.Domain.ExternalReferences;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Intake.Repository.ValueConverters;

internal class StreetRefValueConverter : ValueConverter<StreetRef, Guid>
{
    public StreetRefValueConverter() : base(r => r.Value, value => StreetRef.From(value)) { }
}
