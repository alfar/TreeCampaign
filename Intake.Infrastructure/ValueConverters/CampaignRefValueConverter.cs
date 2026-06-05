using Intake.Domain.ExternalReferences;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Intake.Infrastructure.ValueConverters;

internal class CampaignRefValueConverter : ValueConverter<CampaignRef, Guid>
{
    public CampaignRefValueConverter() : base(r => r.Value, value => CampaignRef.From(value)) { }
}
