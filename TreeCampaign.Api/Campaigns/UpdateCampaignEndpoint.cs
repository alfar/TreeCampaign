using TreeCampaign.Domain.Campaigns;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.ExternalReferences;
using TreeCampaign.Infrastructure;

namespace TreeCampaign.Api.Campaigns;

internal class UpdateCampaignEndpoint
{
    public record UpdateCampaignCommand(CampaignSeason Season, TerritoryRef TerritoryId);

    internal static async Task<IResult> Handle(
        ITreeCampaignUnitOfWork unitOfWork,
        CampaignId campaignId,
        UpdateCampaignCommand command,
        CancellationToken cancellationToken
    )
    {
        var campaign = await unitOfWork.GetRepository<Campaign, CampaignId>().TryFindAsync(campaignId, cancellationToken);

        if (campaign is null)
        {
            return Results.NotFound();
        }
        
        campaign.SetSeason(command.Season);

        if (command.TerritoryId is not null) campaign.SetTerritory(command.TerritoryId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(campaign);
    }
}
