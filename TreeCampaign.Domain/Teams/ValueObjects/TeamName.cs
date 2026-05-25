namespace TreeCampaign.Domain.Teams.ValueObjects;

public sealed record TeamName(string Value)
{
    public static TeamName Empty = new TeamName(string.Empty);

    public static TeamName From(string value) => new TeamName(value);
}
