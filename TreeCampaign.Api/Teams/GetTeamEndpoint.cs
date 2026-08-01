using TreeCampaign.Domain.Teams.ValueObjects;

internal class GetTeamEndpoint
{
    internal static async Task<IResult> Handle(
        ITeamQueries teamQueries,
        TeamId teamId,
        CancellationToken cancellationToken
    )
    {
        var team = await teamQueries.GetTeamAsync(teamId, cancellationToken);
        return team is null ? Results.NotFound() : Results.Ok(team);
    }
}
