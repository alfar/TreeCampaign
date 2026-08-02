using Access.Domain.Users;

namespace Access.Api.Users.Dto;

public sealed record UserDto(Guid Id, string Email, string DisplayName)
{
    public static UserDto From(User user)
    {
        return new UserDto(user.Id.Value, user.Email.Value, user.DisplayName);
    }
}
