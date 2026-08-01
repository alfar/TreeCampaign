namespace Access.Domain.Users.ValueObjects;

public record UserId(Guid Value)
{
    public static bool TryParse(string? input, out UserId userId)
    {
        if (Guid.TryParse(input, out var guid))
        {
            userId = From(guid);
            return true;
        }

        userId = From(Guid.Empty);
        return false;
    }

    public static UserId From(Guid value) => new UserId(value);
}
