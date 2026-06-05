using Intake.Domain.ExternalReferences;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Intake.Infrastructure.ValueConverters;

internal class StreetRefValueConverter : ValueConverter<StreetRef, Guid>
{
    public StreetRefValueConverter() : base(r => r.Value, value => StreetRef.From(value)) { }
}

internal class NullableStreetRefValueConverter : ValueConverter<StreetRef?, Guid?>
{
    public NullableStreetRefValueConverter() : base(r => r != null ? r.Value : null, value => value.HasValue ? StreetRef.From(value.Value) : null) { }
}
