using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.Teams;

public class Team
{
    public required TeamId Id { get; init; }
    public TeamName Name { get; private set; } = TeamName.Empty;
    public required CampaignId CampaignId { get; init; }

    private readonly List<TeamMember> _members = [];
    public IReadOnlyCollection<TeamMember> Members => _members.AsReadOnly();

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

    public void AddMember(string name, string? scoutRelativeName, string phoneNumber)
    {
        _members.Add(new TeamMember(Guid.NewGuid(), name, scoutRelativeName, phoneNumber));
    }

    public void RemoveMember(Guid memberId)
    {
        var member = _members.FirstOrDefault(m => m.Id == memberId);
        if (member is not null)
            _members.Remove(member);
    }
}
