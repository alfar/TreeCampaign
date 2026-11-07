using Common.Infrastructure.Auth;
using TreeCampaign.Api.Helpers;
using TreeCampaign.Domain.Campaigns;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.ExternalReferences;
using TreeCampaign.Infrastructure;

namespace TreeCampaign.Api.Campaigns;

internal class UpdateCampaignEndpoint
{
    public record UpdateCampaignCommand(
        CampaignSeason Season,
        TerritoryRef TerritoryId,
        DateOnly? CollectionDate = null,
        MoneyAmount? PricePerTree = null,
        string? Title = null,
        DateTimeOffset? LastOrderDateTime = null);

    internal static async Task<IResult> Handle(
        ICurrentUserAccessor currentUser,
        ITreeCampaignUnitOfWork unitOfWork,
        CampaignId campaignId,
        UpdateCampaignCommand command,
        CancellationToken cancellationToken
    )
    {
        var campaign = await unitOfWork.GetRepository<Campaign, CampaignId>().TryFindAsync(campaignId, cancellationToken);

        if (campaign is null || campaign.ScoutGroupId != currentUser.GetScoutGroupId())
        {
            return Results.NotFound();
        }
        
        campaign.SetSeason(command.Season);

        if (command.TerritoryId is not null) campaign.SetTerritory(command.TerritoryId);

        // The new optional fields are replaced as a whole, so omitting one (or sending null) clears it.
        campaign.SetCollectionDate(command.CollectionDate);
        campaign.SetPricePerTree(command.PricePerTree);
        campaign.SetTitle(command.Title);
        campaign.SetLastOrderDateTime(command.LastOrderDateTime);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(campaign);
    }
}
