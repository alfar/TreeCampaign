using Intake.Domain.ExternalReferences;
using Intake.InfraStructure.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Intake.Api.Orders;

internal class GetOrdersEndpoint
{
    public static async Task<IResult> Handle([FromRoute] CampaignRef campaignId, IOrderQueries orderQueries, CancellationToken cancellationToken)
    {
        var orders = await orderQueries.GetAllAsync(campaignId, cancellationToken);
        return Results.Ok(orders);
    }
}