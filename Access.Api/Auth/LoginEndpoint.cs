using System.Security.Claims;
using Access.Domain.Users;
using Access.Domain.Users.ValueObjects;
using Access.Infrastructure.Queries;
using Common.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace Access.Api.Auth;

internal class LoginEndpoint
{
    public record LoginRequest(string Email, string Password);

    internal static async Task<IResult> Handle(
        LoginRequest request,
        HttpContext httpContext,
        IUserQueries userQueries,
        IPasswordHasher<User> passwordHasher,
        CancellationToken cancellationToken
    )
    {
        Email email;
        try
        {
            email = Email.Create(request.Email);
        }
        catch (ArgumentException)
        {
            return Results.Unauthorized();
        }

        var user = await userQueries.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return Results.Unauthorized();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
            new(ClaimTypes.Email, user.Email.Value),
            new(ClaimTypes.Name, user.DisplayName),
            new(AccessClaimTypes.ScoutGroupId, user.ScoutGroupId.Value.ToString()),
            new(AccessClaimTypes.PlatformAdmin, user.IsPlatformAdmin.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Results.Ok();
    }
}
