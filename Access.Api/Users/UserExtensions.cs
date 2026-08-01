namespace Access.Api.Users;

public static class UserExtensions
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users").WithTags("Users");

        group.MapGet("/", GetUsersEndpoint.Handle);
        group.MapPost("/", RegisterUserEndpoint.Handle);

        return app;
    }
}
