namespace Access.Api.Auth;

public static class AuthExtensions
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Auth");

        group.MapPost("/login", LoginEndpoint.Handle);
        group.MapPost("/logout", (Delegate)LogoutEndpoint.Handle);
        group.MapGet("/me", MeEndpoint.Handle).RequireAuthorization();

        return app;
    }
}
