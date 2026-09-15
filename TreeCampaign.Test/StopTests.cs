using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.ExternalReferences;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Stops.Events;
using TreeCampaign.Domain.Stops.ValueObjects;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Test;

public class StopTests
{
    private static UnassignedStop CreateUnassignedStop() =>
        UnassignedStop.Create(
            new CampaignId(Guid.NewGuid()),
            new Address("Testvej 1", 0, 0, new StreetSectionRef(Guid.NewGuid())),
            TreeCount.From(2)
        );

    private static UnresolvedStop CreateUnresolvedStop()
    {
        var unassigned = CreateUnassignedStop();
        var assigned = unassigned.AssignToTeam(new TeamId(Guid.NewGuid()));
        return assigned.MarkUnresolved(new ReasonText("Ikke fundet"));
    }

    [Fact]
    public void Abandon_FromUnresolved_ReturnsAbandonedStopWithSameStopData()
    {
        var unresolved = CreateUnresolvedStop();

        var abandoned = unresolved.Abandon();

        Assert.Equal(unresolved.Id, abandoned.Id);
        Assert.Equal(unresolved.CampaignId, abandoned.CampaignId);
        Assert.Equal(unresolved.Address, abandoned.Address);
        Assert.Equal(unresolved.Amount, abandoned.Amount);
    }

    [Fact]
    public void Abandon_FromUnresolved_RaisesStopAbandoned()
    {
        var unresolved = CreateUnresolvedStop();
        unresolved.ClearEvents();

        var abandoned = unresolved.Abandon();

        var raised = Assert.Single(abandoned.NewEvents);
        var stopAbandoned = Assert.IsType<StopAbandoned>(raised);
        Assert.Equal(abandoned.Id, stopAbandoned.Id);
        Assert.Equal(abandoned.CampaignId, stopAbandoned.CampaignId);
    }

    [Fact]
    public void Reopen_FromAbandoned_ReturnsUnassignedStopWithSameStopData()
    {
        var unresolved = CreateUnresolvedStop();
        var abandoned = unresolved.Abandon();

        var unassigned = abandoned.Reopen();

        Assert.Equal(abandoned.Id, unassigned.Id);
        Assert.Equal(abandoned.CampaignId, unassigned.CampaignId);
        Assert.Equal(abandoned.Address, unassigned.Address);
        Assert.Equal(abandoned.Amount, unassigned.Amount);
    }

    [Fact]
    public void Reopen_FromAbandoned_RaisesStopReopened()
    {
        var unresolved = CreateUnresolvedStop();
        var abandoned = unresolved.Abandon();
        abandoned.ClearEvents();

        var unassigned = abandoned.Reopen();

        var raised = Assert.Single(unassigned.NewEvents);
        var stopReopened = Assert.IsType<StopReopened>(raised);
        Assert.Equal(unassigned.Id, stopReopened.Id);
        Assert.Equal(unassigned.CampaignId, stopReopened.CampaignId);
    }
}
