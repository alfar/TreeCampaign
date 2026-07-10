using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams.Events;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Teams;

public sealed class TrailerTeam : TeamBase
{
    public bool IsTrailerFull { get; private set; }

    private TrailerTeam() { }

    public static TrailerTeam Create(CampaignId campaignId, TeamName name)
    {
        return new TrailerTeam
        {
            Id = new TeamId(Guid.NewGuid()),
            Name = name,
            CampaignId = campaignId,
        };
    }

    public void ReportTrailerFull()
    {
        IsTrailerFull = true;
        Raise(new TeamReportedTrailerFull(Id, CampaignId.Value));
    }

    public void ClearTrailerFull()
    {
        if (!IsTrailerFull) return;
        IsTrailerFull = false;
        Raise(new TeamTrailerCleared(Id, CampaignId.Value));
    }
}
