using Common.Infrastructure.Auth;
using TreeCampaign.Api.Helpers;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Stops.ValueObjects;
using TreeCampaign.Infrastructure;

namespace TreeCampaign.Api.Stops;

public class CreateStopEndpoint
{
    public record CreateStopCommand(Address Address, TreeCount Amount);

    public static async Task<IResult> Handle(
        ITreeCampaignUnitOfWork unitOfWork,
        ICurrentUserAccessor currentUser,
        CampaignId campaignId,
        CreateStopCommand command,
        CancellationToken cancellationToken
    )
    {
        if (!await unitOfWork.IsOwnedByCurrentScoutGroupAsync(campaignId, currentUser, cancellationToken))
        {
            return TypedResults.NotFound();
        }

        var stop = UnassignedStop.Create(campaignId, command.Address, command.Amount);

        unitOfWork.GetRepository<UnassignedStop, StopId>().Add(stop);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(ProjectionContext.StopProjection.From(stop));
    }
}
