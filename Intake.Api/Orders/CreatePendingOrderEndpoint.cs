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
        string? SenderName,
        string? SenderPhoneNumber,
        MoneyAmount Amount,
        StreetRef StreetId,
        StreetSectionRef StreetSectionId,
        NeighborhoodRef NeighborhoodId,
        string HouseNumber);

    public record PendingOrderCreatedResponse(
        OrderId Id,
        string PaymentCode,
        MoneyAmount Amount,
        DateTimeOffset OrderDate);

    public static async Task<IResult> Handle(
        IIntakeUnitOfWork unitOfWork,
        CampaignRef campaignId,
        CreatePendingOrderRequest request,
        ICampaignOwnershipService campaignOwnershipService,
        ICurrentUserAccessor currentUser,
        CancellationToken cancellationToken)
    {
        var order = PendingOrder.Create(
            campaignId,
            new Sender(request.SenderName ?? "Anonym", request.SenderPhoneNumber),
            request.Amount,
            request.StreetId,
            request.StreetSectionId,
            request.NeighborhoodId,
            HouseNumber.Parse(request.HouseNumber));

        unitOfWork.GetRepository<PendingOrder, OrderId>().Add(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Created(
            $"/campaigns/{campaignId.Value}/orders/{order.Id.Value}",
            new PendingOrderCreatedResponse(order.Id, order.Message, order.Amount, order.OrderDate));
    }
}

