using Intake.Domain.ExternalReferences;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Intake.InfraStructure.ValueConverters;

internal class StreetSectionRefValueConverter : ValueConverter<StreetSectionRef, Guid>
{
    public StreetSectionRefValueConverter() : base(r => r.Value, value => StreetSectionRef.From(value)) { }
}


internal class NullableStreetSectionRefValueConverter : ValueConverter<StreetSectionRef?, Guid?>
{
    public NullableStreetSectionRefValueConverter() : base(r => r != null ? r.Value : null, value => value.HasValue ? StreetSectionRef.From(value.Value) : null) { }
}
