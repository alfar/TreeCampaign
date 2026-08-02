using Access.Api.Helpers;
using Access.Api.Users.Dto;
using Access.Domain.Users;
using Access.Domain.Users.ValueObjects;
using Access.Infrastructure;
using Common.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;

namespace Access.Api.Users;

internal class RegisterUserEndpoint
{
    public record RegisterUserRequest(string Email, string DisplayName, string Password);

    internal static async Task<IResult> Handle(
        ICurrentUserAccessor userAccessor,
        RegisterUserRequest request,
        IAccessUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        CancellationToken cancellationToken
    )
    {
        var userRepository = unitOfWork.GetRepository<User, UserId>();

        var email = Email.Create(request.Email);
        var passwordHash = passwordHasher.HashPassword(null!, request.Password);

        var user = User.Register(userAccessor.GetScoutGroupId(), email, request.DisplayName, passwordHash);

        userRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok(UserDto.From(user));
    }
}
