using TreeCampaign.Domain.Campaigns.ValueObjects;

internal class GetTeamsEndpoint
{
    internal static async Task<IResult> Handle(
        ITeamQueries teamQueries,
        CampaignId campaignId,
        CancellationToken cancellationToken
    )
    {
        var teams = await teamQueries.GetTeamsAsync(campaignId, cancellationToken);
        return Results.Ok(teams);
    }
}
