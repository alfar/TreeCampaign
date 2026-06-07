using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders;
using Intake.Domain.Orders.ValueObjects;
using Intake.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Intake.Api.Orders;

public static class WashOrderEndpoint
{
    public record WashOrderRequest(StreetRef StreetId, StreetSectionRef StreetSectionId, NeighborhoodRef NeighborhoodId, HouseNumber HouseNumber);

    public static async Task<IResult> Handle([FromRoute] CampaignRef campaignId, [FromRoute] OrderId orderId, WashOrderRequest request, IIntakeUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.GetRepository<UnwashedOrder, OrderId>().TryFindAsync(orderId, cancellationToken);
        if (order is null || order.CampaignId != campaignId)
        {
            return Results.NotFound();
        }

        var newOrder = order.Wash(request.StreetId, request.StreetSectionId, request.NeighborhoodId, request.HouseNumber);
        unitOfWork.GetRepository<UnwashedOrder, OrderId>().Delete(order);
        unitOfWork.GetRepository<WashedOrder, OrderId>().Add(newOrder);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok(newOrder);
    }
}