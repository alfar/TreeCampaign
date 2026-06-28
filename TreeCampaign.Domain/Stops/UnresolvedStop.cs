using System;
using TreeCampaign.Domain.Stops.Events;
using TreeCampaign.Domain.Stops.ValueObjects;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Stops;

public class UnresolvedStop : ReopenableStop
{
    public required ReasonText UnresolvedReason { get; init; }
    public required TeamId UnresolvedByTeamId { get; init; }

    private UnresolvedStop() { }

    public AssignedStop Retry()
    {
        return AssignedStop.CreateFrom(this);
    }

    public AssignedStop Reassign(TeamId teamId)
    {
        return AssignedStop.CreateFrom(this, teamId);
    }

    internal static UnresolvedStop CreateFrom(AssignedStop assignedStop, ReasonText reason)
    {
        var result = new UnresolvedStop
        {
            Id = assignedStop.Id,
            CampaignId = assignedStop.CampaignId,
            Address = assignedStop.Address,
            Amount = assignedStop.Amount,
            UnresolvedReason = reason,
            UnresolvedByTeamId = assignedStop.AssignedTeamId,
        };
        result.Raise(new StopMarkedUnresolved(result.Id, reason, result.CampaignId.Value));
        return result;
    }
}
