using Access.Domain.ScoutGroups;
using Access.Domain.ScoutGroups.ValueObjects;
using Access.Domain.Users;
using Access.Domain.Users.ValueObjects;
using Access.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace Access.Api.ScoutGroups;

internal class CreateScoutGroupEndpoint
{
    public record CreateScoutGroupRequest(string Name, string OwnerEmail, string OwnerDisplayName, string OwnerPassword);

    public record CreateScoutGroupResponse(ScoutGroup ScoutGroup, User Owner);

    internal static async Task<IResult> Handle(
        CreateScoutGroupRequest request,
        IAccessUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        CancellationToken cancellationToken
    )
    {
        var scoutGroupRepository = unitOfWork.GetRepository<ScoutGroup, ScoutGroupId>();
        var userRepository = unitOfWork.GetRepository<User, UserId>();

        var scoutGroup = ScoutGroup.Create(request.Name);

        var ownerEmail = Email.Create(request.OwnerEmail);
        var passwordHash = passwordHasher.HashPassword(null!, request.OwnerPassword);
        var owner = User.Register(scoutGroup.Id, ownerEmail, request.OwnerDisplayName, passwordHash);

        scoutGroupRepository.Add(scoutGroup);
        userRepository.Add(owner);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok(new CreateScoutGroupResponse(scoutGroup, owner));
    }
}
