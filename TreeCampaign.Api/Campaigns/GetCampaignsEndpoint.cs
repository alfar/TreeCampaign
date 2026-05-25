namespace TreeCampaign.Api.Campaigns;

internal class GetCampaignsEndpoint
{
    internal static async Task<IResult> Handle(
        ICampaignQueries campaignQueries,
        CancellationToken cancellationToken
    )
    {
        var campaigns = await campaignQueries.GetCampaignsAsync(cancellationToken);
        return Results.Ok(campaigns);
    }
}
