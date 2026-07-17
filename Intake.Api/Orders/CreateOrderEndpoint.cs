using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders;
using Intake.Domain.Orders.ValueObjects;
using Intake.Infrastructure;

namespace Intake.Api.Orders;

internal class CreateOrderEndpoint
{
    public record CreateOrderRequest(DateTimeOffset OrderDate, string SenderName, string? SenderPhoneNumber, MoneyAmount Amount, string Message);

    public static async Task<IResult> Handle(IIntakeUnitOfWork unitOfWork, CampaignRef campaignId, CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var order = IncomingOrder.Create(campaignId, new Sender(request.SenderName, request.SenderPhoneNumber), request.Amount, request.OrderDate, request.Message);

        unitOfWork.GetRepository<IncomingOrder, OrderId>().Add(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Created($"/campaigns/{campaignId.Value}/orders/{order.Id.Value}", new { Id = order.Id });
    }
}

