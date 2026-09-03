namespace Intake.Api.Orders;

public static class OrderExtensions
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/orders").WithTags("Orders");

        group.MapGet("/", GetOrdersEndpoint.Handle);

        group.MapPost("/", CreateOrderEndpoint.Handle);
        group.MapPost("/{orderId:guid}/wash", WashOrderEndpoint.Handle);
        group.MapPost("/{orderId:guid}/transfer", TransferOrderEndpoint.Handle);
        group.MapDelete("/{orderId:guid}/transfer", UndoTransferOrderEndpoint.Handle);
        group.MapPost("/{orderId:guid}/settle", SettleOrderEndpoint.Handle);
        group.MapPost("/settle-by-territory/{territoryId:guid}", SettleTerritoryOrdersEndpoint.Handle);
        group.MapPost("/import", ImportPaymentsEndpoint.Handle).DisableAntiforgery();
        group.MapPost("/revalidate", RevalidateCampaignOrdersEndpoint.Handle);

        return app;
    }
}
