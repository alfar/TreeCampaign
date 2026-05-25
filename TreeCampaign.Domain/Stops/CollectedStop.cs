using System;
using TreeCampaign.Domain.Stops.Events;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Stops;

public class CollectedStop : ReopenableStop
{
    public required TeamId CollectedByTeamId { get; init; }

    internal CollectedStop() { }

    public AssignedStop CorrectMistakenCollection()
    {
        return AssignedStop.CreateFrom(this);
    }

    internal static CollectedStop CreateFrom(AssignedStop assignedStop)
    {
        var result = new CollectedStop
        {
            Id = assignedStop.Id,
            CampaignId = assignedStop.CampaignId,
            Address = assignedStop.Address,
            Amount = assignedStop.Amount,
            CollectedByTeamId = assignedStop.AssignedTeamId,
        };
        result.Raise(new StopCollected(result.Id));
        return result;
    }
}
