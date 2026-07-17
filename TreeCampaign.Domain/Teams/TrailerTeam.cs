using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams.Events;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Teams;

public sealed class TrailerTeam : TeamBase
{
    public bool IsTrailerFull { get; private set; }
    public TrailerSize TrailerSize { get; private set; }

    private TrailerTeam()
    {
    }

    public static TrailerTeam Create(CampaignId campaignId, TeamName name, TrailerSize trailerSize)
    {
        var team = new TrailerTeam
        {
            Id = new TeamId(Guid.NewGuid()),
            Name = name,
            CampaignId = campaignId,
            TrailerSize = trailerSize,
        };
        team.Raise(new TeamCreated(team.Id, team.CampaignId, team.Name, TeamKind.Trailer));

        return team;
    }

    public void SetTrailerSize(TrailerSize trailerSize)
    {
        if (TrailerSize == trailerSize) return;
        TrailerSize = trailerSize;
        Raise(new TeamTrailerSizeUpdated(Id, CampaignId, trailerSize));
    }

    public void ReportTrailerFull()
    {
        IsTrailerFull = true;
        Raise(new TeamReportedTrailerFull(Id, CampaignId));
    }

    public void ClearTrailerFull()
    {
        if (!IsTrailerFull) return;
        IsTrailerFull = false;
        Raise(new TeamTrailerCleared(Id, CampaignId));
    }
}
