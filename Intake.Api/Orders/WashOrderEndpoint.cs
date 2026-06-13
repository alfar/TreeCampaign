using Common.Infrastructure.Abstractions;
using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;
using Intake.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Intake.Api.Orders;

public static class WashOrderEndpoint
{
    public record WashOrderRequest(StreetRef StreetId, HouseNumber HouseNumber);

    public static async Task<IResult> Handle([FromRoute] CampaignRef campaignId, [FromRoute] OrderId orderId, WashOrderRequest request, ISectionResolutionService sectionResolutionService, IIntakeUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.GetRepository<UnwashedOrder, OrderId>().TryFindAsync(orderId, cancellationToken);
        if (order is null || order.CampaignId != campaignId)
        {
            return Results.NotFound();
        }

        var result = await sectionResolutionService.ResolveSectionAsync(campaignId, request.StreetId, request.HouseNumber, cancellationToken);

        switch (result)
        {
            case SuccessfulSectionResolutionResult success:
                var newOrder = order.Wash(request.StreetId, success.StreetSectionId, success.NeighborhoodId, request.HouseNumber);
                unitOfWork.Transition<UnwashedOrder, WashedOrder, OrderId>(order, newOrder);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok(newOrder);
            case OutOfBoundsSectionResolutionResult:
                var oobOrder = order.MarkOutOfBounds(request.StreetId, request.HouseNumber);
                unitOfWork.Transition<UnwashedOrder, OutOfBoundsOrder, OrderId>(order, oobOrder);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Results.Ok(oobOrder);
            default:
                return Results.UnprocessableEntity();
        }
    }
}