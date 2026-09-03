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

public static class MarkOrderUnwashableEndpoint
{
    public static async Task<IResult> Handle(
        [FromRoute] CampaignRef campaignId,
        [FromRoute] OrderId orderId,
        IIntakeUnitOfWork unitOfWork,
        ICampaignOwnershipService campaignOwnershipService,
        ICurrentUserAccessor currentUser,
        CancellationToken cancellationToken)
    {
        if (!await campaignOwnershipService.IsOwnedByCurrentScoutGroupAsync(campaignId, currentUser, cancellationToken))
        {
            return Results.NotFound();
        }

        var unwashedOrder = await unitOfWork.GetRepository<UnwashedOrder, OrderId>().TryFindAsync(orderId, cancellationToken);
        if (unwashedOrder is not null && unwashedOrder.CampaignId == campaignId)
        {
            var newOrder = unwashedOrder.MarkUnwashable();
            unitOfWork.Transition<UnwashedOrder, UnwashableOrder, OrderId>(unwashedOrder, newOrder);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Results.Ok(newOrder);
        }

        var outOfBoundsOrder = await unitOfWork.GetRepository<OutOfBoundsOrder, OrderId>().TryFindAsync(orderId, cancellationToken);
        if (outOfBoundsOrder is not null && outOfBoundsOrder.CampaignId == campaignId)
        {
            var newOrder = outOfBoundsOrder.MarkUnwashable();
            unitOfWork.Transition<OutOfBoundsOrder, UnwashableOrder, OrderId>(outOfBoundsOrder, newOrder);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Results.Ok(newOrder);
        }

        return Results.NotFound();
    }
}
