using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Teams;

public sealed class WalkingTeam : TeamBase
{
    private WalkingTeam() { }

    public static WalkingTeam Create(CampaignId campaignId, TeamName name)
    {
        return new WalkingTeam
        {
            Id = new TeamId(Guid.NewGuid()),
            Name = name,
            CampaignId = campaignId,
        };
    }
}
