using System;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops;
using Common.Repository.Abstractions;

namespace TreeCampaign.Api.Stops;

public class CreateStopEndpoint
{
    public record CreateStopCommand(Address Address, TreeCount Amount);

    public static async Task<IResult> Handle(
        IUnitOfWork unitOfWork,
        CampaignId campaignId,
        CreateStopCommand command
    )
    {
        var stop = UnassignedStop.Create(campaignId, command.Address, command.Amount);

        unitOfWork.GetRepository<UnassignedStop, StopId>().Add(stop);
        await unitOfWork.SaveChangesAsync();

        return TypedResults.Ok(ProjectionContext.StopProjection.From(stop));
    }
}
