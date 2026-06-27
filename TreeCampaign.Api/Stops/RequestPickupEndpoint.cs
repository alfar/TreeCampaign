using TreeCampaign.Application;
using TreeCampaign.Domain.Campaigns.ValueObjects;

namespace TreeCampaign.Api.Stops;

internal class RequestPickupEndpoint
{
    internal record RequestPickupCommand(Guid StreetId, string HouseNumber, int TreeCount);

    internal static async Task<IResult> Handle(
        PickupRequestService pickupRequestService,
        CampaignId campaignId,
        RequestPickupCommand command,
        CancellationToken cancellationToken)
    {
        var stop = await pickupRequestService.RequestPickupAsync(
            campaignId, command.StreetId, command.HouseNumber, command.TreeCount, cancellationToken);

        if (stop is null)
            return TypedResults.BadRequest("Address could not be validated in the current territory.");

        return TypedResults.Ok(stop);
    }
}
