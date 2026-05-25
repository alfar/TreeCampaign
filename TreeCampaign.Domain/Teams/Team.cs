using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Teams;

public class Team
{
    public required TeamId Id { get; init; }
    public TeamName Name { get; private set; } = TeamName.Empty;
    public required CampaignId CampaignId { get; init; }

    private Team() { }

    public static Team Create(CampaignId campaignId, TeamName name)
    {
        return new Team
        {
            Id = new TeamId(Guid.NewGuid()),
            Name = name,
            CampaignId = campaignId,
        };
    }

    public void UpdateName(TeamName name)
    {
        Name = name;
    }
}
