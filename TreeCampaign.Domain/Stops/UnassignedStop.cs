using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops.Events;
using TreeCampaign.Domain.Stops.ValueObjects;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Stops;

public class UnassignedStop : StopBase
{
    public static UnassignedStop Create(CampaignId campaignId, Address address, TreeCount amount)
    {
        var result = new UnassignedStop
        {
            Id = new StopId(Guid.NewGuid()),
            CampaignId = campaignId,
            Address = address,
            Amount = amount,
        };

        result.Raise(new StopCreated(result.Id, address, amount));

        return result;
    }

    private UnassignedStop() { }

    public AssignedStop AssignToTeam(TeamId teamId)
    {
        return AssignedStop.CreateFrom(this, teamId);
    }

    internal static UnassignedStop CreateFrom(AssignedStop assignedStop)
    {
        var result = new UnassignedStop
        {
            Id = assignedStop.Id,
            CampaignId = assignedStop.CampaignId,
            Address = assignedStop.Address,
            Amount = assignedStop.Amount,
        };

        result.Raise(new StopUnassigned(result.Id));

        return result;
    }

    internal static UnassignedStop CreateFrom(ReopenableStop reopenableStop)
    {
        var result = new UnassignedStop
        {
            Id = reopenableStop.Id,
            CampaignId = reopenableStop.CampaignId,
            Address = reopenableStop.Address,
            Amount = reopenableStop.Amount,
        };

        result.Raise(new StopReopened(result.Id));

        return result;
    }
}
