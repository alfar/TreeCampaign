namespace Intake.Domain.ExternalReferences;

public record NeighborhoodRef(Guid Value)
{
    public static NeighborhoodRef From(Guid value) => new NeighborhoodRef(value);
}