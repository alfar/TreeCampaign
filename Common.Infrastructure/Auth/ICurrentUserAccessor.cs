namespace Common.Infrastructure.Auth;

public interface ICurrentUserAccessor
{
    Guid? ScoutGroupId { get; }
    bool IsPlatformAdmin { get; }
}
