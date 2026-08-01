using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Access.Api.Auth;

internal class LogoutEndpoint
{
    internal static async Task<IResult> Handle(HttpContext httpContext)
    {
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Results.Ok();
    }
}
