using System;
using TreeCampaign.Domain.Stops.Events;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Stops;

public class AssignedStop : StopBase
{
    public required TeamId AssignedTeamId { get; init; }

    private AssignedStop() { }

    public UnassignedStop Unassign()
    {
        return UnassignedStop.CreateFrom(this);
    }

    public CollectedStop Collect()
    {
        return CollectedStop.CreateFrom(this);
    }

    public UnresolvedStop MarkUnresolved(ReasonText reason)
    {
        return UnresolvedStop.CreateFrom(this, reason);
    }

    internal static AssignedStop CreateFrom(UnassignedStop unassignedStop, TeamId teamId)
    {
        var result = new AssignedStop
        {
            Id = unassignedStop.Id,
            CampaignId = unassignedStop.CampaignId,
            Address = unassignedStop.Address,
            Amount = unassignedStop.Amount,
            AssignedTeamId = teamId,
        };

        result.Raise(new StopAssigned(result.Id, teamId));

        return result;
    }

    internal static AssignedStop CreateFrom(CollectedStop collectedStop)
    {
        var result = new AssignedStop
        {
            Id = collectedStop.Id,
            CampaignId = collectedStop.CampaignId,
            Address = collectedStop.Address,
            Amount = collectedStop.Amount,
            AssignedTeamId = collectedStop.CollectedByTeamId,
        };

        result.Raise(new StopCollectionCorrected(result.Id));

        return result;
    }

    internal static AssignedStop CreateFrom(UnresolvedStop unresolvedStop, TeamId teamId)
    {
        var result = new AssignedStop
        {
            Id = unresolvedStop.Id,
            CampaignId = unresolvedStop.CampaignId,
            Address = unresolvedStop.Address,
            Amount = unresolvedStop.Amount,
            AssignedTeamId = teamId,
        };

        result.Raise(new StopReassigned(result.Id, teamId));

        return result;
    }

    internal static AssignedStop CreateFrom(UnresolvedStop unresolvedStop)
    {
        var result = new AssignedStop
        {
            Id = unresolvedStop.Id,
            CampaignId = unresolvedStop.CampaignId,
            Address = unresolvedStop.Address,
            Amount = unresolvedStop.Amount,
            AssignedTeamId = unresolvedStop.UnresolvedByTeamId,
        };

        result.Raise(new StopRetried(result.Id));

        return result;
    }
}
