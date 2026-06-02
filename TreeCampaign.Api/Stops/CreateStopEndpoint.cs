using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.InfraStructure;

namespace TreeCampaign.Api.Stops;

public class CreateStopEndpoint
{
    public record CreateStopCommand(Address Address, TreeCount Amount);

    public static async Task<IResult> Handle(
        ITreeCampaignUnitOfWork unitOfWork,
        CampaignId campaignId,
        CreateStopCommand command,
        CancellationToken cancellationToken
    )
    {
        var stop = UnassignedStop.Create(campaignId, command.Address, command.Amount);

        unitOfWork.GetRepository<UnassignedStop, StopId>().Add(stop);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(ProjectionContext.StopProjection.From(stop));
    }
}
