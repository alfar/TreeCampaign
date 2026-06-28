using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Stops;

public class DeliveredStop : StopBase
{
    public required TeamId DeliveredByTeamId { get; init; }

    private DeliveredStop() { }

    internal static DeliveredStop CreateFrom(CollectedStop collectedStop)
    {
        var result = new DeliveredStop
        {
            Id = collectedStop.Id,
            CampaignId = collectedStop.CampaignId,
            Address = collectedStop.Address,
            Amount = collectedStop.Amount,
            DeliveredByTeamId = collectedStop.CollectedByTeamId,
        };
        result.Raise(new Events.StopDelivered(result.Id, result.CampaignId.Value));
        return result;
    }
}
