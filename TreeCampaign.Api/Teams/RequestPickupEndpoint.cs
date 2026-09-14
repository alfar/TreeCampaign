using TreeCampaign.Application;
using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams.ValueObjects;

internal class RequestPickupEndpoint
{
    internal record RequestPickupCommand(Guid StreetId, string HouseNumber, int TreeCount);

    internal static async Task<IResult> Handle(
        PickupRequestService pickupRequestService,
        CampaignId campaignId,
        TeamId teamId,
        RequestPickupCommand command,
        CancellationToken cancellationToken)
    {
        var result = await pickupRequestService.RequestPickupAsync(
            campaignId, teamId, command.StreetId, command.HouseNumber, command.TreeCount, cancellationToken);

        if (result is null)
            return TypedResults.BadRequest("Address could not be validated in the current territory.");

        return TypedResults.Ok(result);
    }
}
