using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Stops.ValueObjects;
using TreeCampaign.Domain.Teams;
using TreeCampaign.Domain.Teams.Events;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Test;

public class TeamExtraTreesTests
{
    private static WalkingTeam CreateTeam() =>
        WalkingTeam.Create(new CampaignId(Guid.NewGuid()), TeamName.From("Test Team"));

    [Fact]
    public void AdjustExtraTrees_PositiveDelta_IncreasesCurrentAndTotal()
    {
        var team = CreateTeam();

        team.AdjustExtraTrees(TreeCountDelta.From(3));

        Assert.Equal(3, team.CurrentExtraTrees.Value);
        Assert.Equal(3, team.TotalExtraTrees.Value);
    }

    [Fact]
    public void AdjustExtraTrees_NegativeDelta_DecreasesCurrentAndTotal()
    {
        var team = CreateTeam();
        team.AdjustExtraTrees(TreeCountDelta.From(5));

        team.AdjustExtraTrees(TreeCountDelta.From(-2));

        Assert.Equal(3, team.CurrentExtraTrees.Value);
        Assert.Equal(3, team.TotalExtraTrees.Value);
    }

    [Fact]
    public void AdjustExtraTrees_NegativeDeltaBelowZero_ClampsCurrentAndTotalByActualDeltaOnly()
    {
        var team = CreateTeam();
        team.AdjustExtraTrees(TreeCountDelta.From(2));

        team.AdjustExtraTrees(TreeCountDelta.From(-5));

        Assert.Equal(0, team.CurrentExtraTrees.Value);
        Assert.Equal(0, team.TotalExtraTrees.Value);
    }

    [Fact]
    public void AdjustExtraTrees_ZeroActualDelta_DoesNotRaiseEvent()
    {
        var team = CreateTeam();
        team.ClearEvents();

        team.AdjustExtraTrees(TreeCountDelta.From(-1));

        Assert.Equal(0, team.CurrentExtraTrees.Value);
        Assert.Empty(team.NewEvents);
    }

    [Fact]
    public void AdjustExtraTrees_NonZeroActualDelta_RaisesTeamExtraTreesAdjusted()
    {
        var team = CreateTeam();
        team.ClearEvents();

        team.AdjustExtraTrees(TreeCountDelta.From(4));

        var raised = Assert.Single(team.NewEvents);
        var adjusted = Assert.IsType<TeamExtraTreesAdjusted>(raised);
        Assert.Equal(4, adjusted.Delta.Value);
        Assert.Equal(4, adjusted.CurrentExtraTrees.Value);
        Assert.Equal(4, adjusted.TotalExtraTrees.Value);
    }

    [Fact]
    public void ResetCurrentExtraTrees_LeavesTotalUnaffected()
    {
        var team = CreateTeam();
        team.AdjustExtraTrees(TreeCountDelta.From(6));

        team.ResetCurrentExtraTrees();

        Assert.Equal(0, team.CurrentExtraTrees.Value);
        Assert.Equal(6, team.TotalExtraTrees.Value);
    }

    [Fact]
    public void ResetCurrentExtraTrees_WhenAlreadyZero_IsNoOpAndRaisesNoEvent()
    {
        var team = CreateTeam();
        team.ClearEvents();

        team.ResetCurrentExtraTrees();

        Assert.Empty(team.NewEvents);
    }

    [Fact]
    public void ResetCurrentExtraTrees_WhenNonZero_RaisesTeamExtraTreesReset()
    {
        var team = CreateTeam();
        team.AdjustExtraTrees(TreeCountDelta.From(2));
        team.ClearEvents();

        team.ResetCurrentExtraTrees();

        var raised = Assert.Single(team.NewEvents);
        var reset = Assert.IsType<TeamExtraTreesReset>(raised);
        Assert.Equal(2, reset.TotalExtraTrees.Value);
    }

    [Theory]
    [InlineData(0, 3, 3)]
    [InlineData(5, -2, 3)]
    [InlineData(2, -10, 0)]
    public void Adjust_TreeCount_ClampsAtZero(int startValue, int delta, int expected)
    {
        var count = TreeCount.From(startValue);

        var adjusted = count.Adjust(TreeCountDelta.From(delta));

        Assert.Equal(expected, adjusted.Value);
    }
}
