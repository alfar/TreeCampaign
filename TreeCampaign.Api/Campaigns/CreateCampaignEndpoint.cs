using Common.Infrastructure.Auth;
using TreeCampaign.Api.Helpers;
using TreeCampaign.Domain.Campaigns;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.ExternalReferences;
using TreeCampaign.Infrastructure;

namespace TreeCampaign.Api.Campaigns;

internal class CreateCampaignEndpoint
{
    public record CreateCampaignCommand(CampaignSeason Season, TerritoryRef? TerritoryId);

    internal static async Task<IResult> Handle(
        ICurrentUserAccessor currentUser,
        ITreeCampaignUnitOfWork unitOfWork,
        CreateCampaignCommand command,
        CancellationToken cancellationToken
    )
    {
        var campaign = Campaign.Create(command.Season, currentUser.GetScoutGroupId(), command.TerritoryId);

        unitOfWork.GetRepository<Campaign, CampaignId>().Add(campaign);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(campaign);
    }
}
