using Intake.Domain.ExternalReferences;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Intake.Repository.ValueConverters;

internal class StreetSectionRefValueConverter : ValueConverter<StreetSectionRef, Guid>
{
    public StreetSectionRefValueConverter() : base(r => r.Value, value => StreetSectionRef.From(value)) { }
}
