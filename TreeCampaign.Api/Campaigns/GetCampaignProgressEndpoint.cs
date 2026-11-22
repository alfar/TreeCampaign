using TreeCampaign.Domain.Campaigns.ValueObjects;
using TreeCampaign.Infrastructure.Queries;

namespace TreeCampaign.Api.Campaigns;

/// <summary>
/// Public, aggregate-only tree counts for the progress pie. Exposes no addresses or other per-stop data.
/// </summary>
internal class GetCampaignProgressEndpoint
{
    internal record CampaignProgress(
        int Unassigned,
        int Assigned,
        int Unresolved,
        int Collected,
        int Extra,
        int Total
    );

    internal static async Task<IResult> Handle(
        Guid campaignId,
        ICampaignQueries campaignQueries,
        IStopQueries stopQueries,
        ITeamQueries teamQueries,
        CancellationToken cancellationToken
    )
    {
        var id = CampaignId.From(campaignId);
        if (await campaignQueries.GetByIdAsync(id, cancellationToken) is null)
        {
            return Results.NotFound();
        }

        var counts = await stopQueries.GetTreeCountsByStopTypeAsync(id, cancellationToken);
        int Count(params IStopQueries.State[] states) =>
            states.Sum(s => counts.GetValueOrDefault(s.ToString()));

        var unassigned = Count(IStopQueries.State.Unassigned);
        var assigned = Count(IStopQueries.State.Assigned);
        var unresolved = Count(IStopQueries.State.Unresolved, IStopQueries.State.Abandoned);
        var collected = Count(IStopQueries.State.Collected, IStopQueries.State.Delivered);
        var extra = await teamQueries.GetTotalExtraTreesAsync(id, cancellationToken);

        return Results.Ok(
            new CampaignProgress(
                unassigned,
                assigned,
                unresolved,
                collected,
                extra,
                unassigned + assigned + unresolved + collected + extra
            )
        );
    }
}
