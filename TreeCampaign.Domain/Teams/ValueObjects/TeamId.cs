namespace TreeCampaign.Domain.Teams.ValueObjects;

public sealed record TeamId(Guid Value)
{
    public static bool TryParse(string? input, out TeamId teamId)
    {
        if (Guid.TryParse(input, out var guid))
        {
            teamId = new TeamId(guid);
            return true;
        }

        teamId = new TeamId(Guid.Empty);
        return false;
    }

    public static TeamId From(Guid value) => new TeamId(value);
}
