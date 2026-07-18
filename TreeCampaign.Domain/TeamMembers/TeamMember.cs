using TreeCampaign.Domain.TeamMembers.ValueObjects;
using TreeCampaign.Domain.Teams.ValueObjects;

namespace TreeCampaign.Domain.TeamMembers;

public class TeamMember
{
    public required TeamMemberId Id { get; init; }
    public required string Name { get; init; }
    public string? ScoutRelativeName { get; init; }
    public string? PhoneNumber { get; init; }
    public required TeamId TeamId { get; init; }

}
