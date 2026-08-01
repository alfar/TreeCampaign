using Microsoft.AspNetCore.Http;

namespace Common.Infrastructure.Auth;

internal class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    public Guid? ScoutGroupId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirst(AccessClaimTypes.ScoutGroupId)?.Value;
            return Guid.TryParse(value, out var guid) ? guid : null;
        }
    }

    public bool IsPlatformAdmin
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirst(AccessClaimTypes.PlatformAdmin)?.Value;
            return bool.TryParse(value, out var isPlatformAdmin) && isPlatformAdmin;
        }
    }
}
