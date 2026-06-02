using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeCampaign.Domain.Campaigns.ValueObjects;

internal class CampaignSeasonValueConverter : ValueConverter<CampaignSeason, int>
{
    public CampaignSeasonValueConverter()
        : base(season => season.Year, value => new CampaignSeason(value)) { }
}
