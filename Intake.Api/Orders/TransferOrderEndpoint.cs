using Common.Infrastructure.Abstractions;
using Common.Infrastructure.Auth;
using Intake.Api.Helpers;
using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;
using Intake.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Intake.Api.Orders;

public static class TransferOrderEndpoint
{
    public record TransferOrderRequest(TerritoryRef TerritoryId);

    public static async Task<IResult> Handle(
        [FromRoute] CampaignRef campaignId,
        [FromRoute] OrderId orderId,
        TransferOrderRequest request,
        IIntakeUnitOfWork unitOfWork,
        IAddressValidationService addressValidationService,
        ICampaignOwnershipService campaignOwnershipService,
        ICurrentUserAccessor currentUser,
        CancellationToken cancellationToken)
    {
        if (!await campaignOwnershipService.IsOwnedByCurrentScoutGroupAsync(campaignId, currentUser, cancellationToken))
        {
            return Results.NotFound();
        }

        var order = await unitOfWork.GetRepository<OutOfBoundsOrder, OrderId>().TryFindAsync(orderId, cancellationToken);
        if (order is null || order.CampaignId != campaignId)
        {
            return Results.NotFound();
        }

        if (!await addressValidationService.DoesTerritoryExistAsync(request.TerritoryId, cancellationToken))
        {
            return Results.BadRequest("The specified territory does not exist.");
        }

        var newOrder = order.Transfer(request.TerritoryId);
        unitOfWork.Transition<OutOfBoundsOrder, TransferredOrder, OrderId>(order, newOrder);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok(newOrder);
    }
}