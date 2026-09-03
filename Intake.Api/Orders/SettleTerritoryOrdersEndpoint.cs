using Common.Infrastructure.Abstractions;
using Common.Infrastructure.Auth;
using Intake.Api.Helpers;
using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;
using Intake.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Intake.Api.Orders;

public static class SettleTerritoryOrdersEndpoint
{
    public static async Task<IResult> Handle(
        [FromRoute] CampaignRef campaignId,
        [FromRoute] Guid territoryId,
        IIntakeUnitOfWork unitOfWork,
        ICampaignOwnershipService campaignOwnershipService,
        ICurrentUserAccessor currentUser,
        CancellationToken cancellationToken)
    {
        if (!await campaignOwnershipService.IsOwnedByCurrentScoutGroupAsync(campaignId, currentUser, cancellationToken))
        {
            return Results.NotFound();
        }

        var territoryRef = TerritoryRef.From(territoryId);
        var orders = await unitOfWork.GetTransferredOrdersByTerritory(campaignId, territoryRef).ToListAsync(cancellationToken);

        var settledOrders = new List<SettledOrder>();
        foreach (var order in orders)
        {
            var settled = order.MarkAsPaid();
            unitOfWork.Transition<TransferredOrder, SettledOrder, OrderId>(order, settled);
            settledOrders.Add(settled);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok(settledOrders);
    }
}
