using Access.Domain.ScoutGroups.ValueObjects;

namespace Access.Domain.ScoutGroups;

public sealed class ScoutGroup
{
    public required ScoutGroupId Id { get; init; }
    public required string Name { get; init; }

    private ScoutGroup() { }

    public static ScoutGroup Create(string name)
    {
        return new ScoutGroup
        {
            Id = ScoutGroupId.From(Guid.NewGuid()),
            Name = name
        };
    }
}
