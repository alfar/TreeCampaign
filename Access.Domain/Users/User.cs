using Access.Domain.ScoutGroups.ValueObjects;
using Access.Domain.Users.ValueObjects;

namespace Access.Domain.Users;

public sealed class User
{
    public required UserId Id { get; init; }
    public required ScoutGroupId ScoutGroupId { get; init; }
    public required Email Email { get; init; }
    public required string DisplayName { get; init; }
    public string PasswordHash { get; private set; }
    public bool IsPlatformAdmin { get; private set; }

    private User() { PasswordHash = default!; }

    public static User Register(ScoutGroupId scoutGroupId, Email email, string displayName, string passwordHash)
    {
        return new User
        {
            Id = UserId.From(Guid.NewGuid()),
            ScoutGroupId = scoutGroupId,
            Email = email,
            DisplayName = displayName,
            PasswordHash = passwordHash,
            IsPlatformAdmin = false
        };
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }

    public void GrantPlatformAdmin()
    {
        IsPlatformAdmin = true;
    }

    public void RevokePlatformAdmin()
    {
        IsPlatformAdmin = false;
    }
}
