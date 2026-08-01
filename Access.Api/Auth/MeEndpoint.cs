using System.Security.Claims;

namespace Access.Api.Auth;

internal class MeEndpoint
{
    public record CurrentUserResponse(Guid UserId, string Email, string DisplayName, Guid ScoutGroupId, bool IsPlatformAdmin);

    internal static IResult Handle(ClaimsPrincipal user)
    {
        var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var email = user.FindFirstValue(ClaimTypes.Email)!;
        var displayName = user.FindFirstValue(ClaimTypes.Name)!;
        var scoutGroupId = Guid.Parse(user.FindFirstValue(AccessClaimTypes.ScoutGroupId)!);
        var isPlatformAdmin = bool.Parse(user.FindFirstValue(AccessClaimTypes.PlatformAdmin)!);

        return Results.Ok(new CurrentUserResponse(userId, email, displayName, scoutGroupId, isPlatformAdmin));
    }
}
