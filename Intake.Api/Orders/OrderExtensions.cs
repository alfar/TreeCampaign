namespace Intake.Api.Orders;

public static class OrderExtensions
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/orders").WithTags("Orders");

        group.MapGet("/", GetOrdersEndpoint.Handle);

        group.MapPost("/", CreateOrderEndpoint.Handle);
        group.MapPost("/{orderId:guid}/wash", WashOrderEndpoint.Handle);
        group.MapPost("/import", ImportPaymentsEndpoint.Handle).DisableAntiforgery();

        return app;
    }
}
