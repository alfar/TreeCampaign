using Common.Repository.Abstractions;
using TreeCampaign.Domain.Campaigns;
using TreeCampaign.Domain.Campaigns.ValueObjects;

namespace TreeCampaign.Api.Campaigns;

internal class CreateCampaignEndpoint
{
    public record CreateCampaignCommand(CampaignSeason Season);

    internal static async Task<IResult> Handle(
        IUnitOfWork unitOfWork,
        CreateCampaignCommand command
    )
    {
        var campaign = Campaign.Create(command.Season);

        unitOfWork.GetRepository<Campaign, CampaignId>().Add(campaign);
        await unitOfWork.SaveChangesAsync();

        return TypedResults.Ok(campaign);
    }
}
