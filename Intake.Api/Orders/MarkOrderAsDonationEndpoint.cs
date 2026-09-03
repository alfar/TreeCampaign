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

public static class MarkOrderAsDonationEndpoint
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

        var order = await unitOfWork.GetRepository<UnwashableOrder, OrderId>().TryFindAsync(orderId, cancellationToken);
        if (order is null || order.CampaignId != campaignId)
        {
            return Results.NotFound();
        }

        var newOrder = order.MarkAsDonation();
        unitOfWork.Transition<UnwashableOrder, DonatedOrder, OrderId>(order, newOrder);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok(newOrder);
    }
}
