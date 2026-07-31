using Common.Infrastructure.Abstractions;
using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;
using Intake.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Intake.Api.Orders;

public static class UndoTransferOrderEndpoint
{
    public static async Task<IResult> Handle([FromRoute] CampaignRef campaignId, [FromRoute] OrderId orderId, IIntakeUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.GetRepository<TransferredOrder, OrderId>().TryFindAsync(orderId, cancellationToken);
        if (order is null || order.CampaignId != campaignId)
        {
            return Results.NotFound();
        }

        var newOrder = order.UndoTransfer();
        unitOfWork.Transition<TransferredOrder, OutOfBoundsOrder, OrderId>(order, newOrder);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok(newOrder);
    }
}