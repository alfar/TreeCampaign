using Common.Infrastructure.Auth;
using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;
using Intake.Infrastructure;

namespace Intake.Api.Orders;

internal class CreatePendingOrderEndpoint
{
    public record CreatePendingOrderRequest(
        DateTimeOffset OrderDate,
        string SenderName,
        string? SenderPhoneNumber,
        MoneyAmount Amount,
        string Message,
        TransactionId TransactionId,
        StreetRef StreetId,
        StreetSectionRef StreetSectionId,
        NeighborhoodRef NeighborhoodId,
        string HouseNumber);

    public static async Task<IResult> Handle(
        IIntakeUnitOfWork unitOfWork,
        CampaignRef campaignId,
        CreatePendingOrderRequest request,
        ICampaignOwnershipService campaignOwnershipService,
        ICurrentUserAccessor currentUser,
        CancellationToken cancellationToken)
    {
        var order = PendingOrder.Create(campaignId, new Sender(request.SenderName, request.SenderPhoneNumber), request.Amount, request.OrderDate, request.Message, request.TransactionId, request.StreetId, request.StreetSectionId, request.NeighborhoodId, HouseNumber.Parse(request.HouseNumber));

        unitOfWork.GetRepository<PendingOrder, OrderId>().Add(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Created($"/campaigns/{campaignId.Value}/orders/{order.Id.Value}", new { Id = order.Id });
    }
}

