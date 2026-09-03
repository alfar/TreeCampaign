using Common.Infrastructure.Auth;
using Intake.Api.Helpers;
using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;
using Intake.Infrastructure.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Intake.Api.Orders;

internal class GetOrdersEndpoint
{
    public static async Task<IResult> Handle(
        [FromRoute] CampaignRef campaignId,
        IOrderQueries orderQueries,
        ICampaignOwnershipService campaignOwnershipService,
        ICurrentUserAccessor currentUser,
        CancellationToken cancellationToken)
    {
        if (!await campaignOwnershipService.IsOwnedByCurrentScoutGroupAsync(campaignId, currentUser, cancellationToken))
        {
            return Results.NotFound();
        }

        var orders = await orderQueries.GetAllAsync(campaignId, cancellationToken);
        return Results.Ok(orders);
    }
}