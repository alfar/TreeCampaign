using TreeCampaign.Domain.Stops.Events;

namespace TreeCampaign.Domain.Stops;

public class AbandonedStop : ReopenableStop
{
    private AbandonedStop() { }

    internal static AbandonedStop CreateFrom(UnresolvedStop unresolvedStop)
    {
        var result = new AbandonedStop
        {
            Id = unresolvedStop.Id,
            CampaignId = unresolvedStop.CampaignId,
            Address = unresolvedStop.Address,
            Amount = unresolvedStop.Amount,
        };
        result.Raise(new StopAbandoned(result.Id, result.CampaignId));
        return result;
    }
}
