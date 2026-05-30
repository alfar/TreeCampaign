using Common.Repository.Abstractions;
using TreeCampaign.Domain.Campaigns;
using TreeCampaign.Domain.Campaigns.ValueObjects;

namespace TreeCampaign.Api.Campaigns;

internal class CreateCampaignEndpoint
{
    public record CreateCampaignCommand(CampaignSeason Season);

    internal static async Task<IResult> Handle(
        IUnitOfWork unitOfWork,
        CreateCampaignCommand command,
        CancellationToken cancellationToken
    )
    {
        var campaign = Campaign.Create(command.Season);

        unitOfWork.GetRepository<Campaign, CampaignId>().Add(campaign);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(campaign);
    }
}
