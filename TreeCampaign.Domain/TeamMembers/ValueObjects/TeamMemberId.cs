namespace TreeCampaign.Domain.TeamMembers.ValueObjects;

public sealed record TeamMemberId(Guid Value)
{
    public static bool TryParse(string? input, out TeamMemberId TeamMemberId)
    {
        if (Guid.TryParse(input, out var guid))
        {
            TeamMemberId = new TeamMemberId(guid);
            return true;
        }

        TeamMemberId = new TeamMemberId(Guid.Empty);
        return false;
    }

    public static TeamMemberId From(Guid value) => new TeamMemberId(value);
}
