using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeCampaign.Domain.Campaigns.ValueObjects;

namespace TreeCampaign.InfraStructure.ValueConverters;

internal class CampaignIdValueConverter : ValueConverter<CampaignId, Guid>
{
    public CampaignIdValueConverter()
        : base(id => id.Value, value => CampaignId.From(value)) { }
}
