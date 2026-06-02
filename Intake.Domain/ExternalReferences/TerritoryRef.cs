namespace Intake.Domain.ExternalReferences;

public record TerritoryRef(Guid Value)
{
    public static TerritoryRef From(Guid value) => new TerritoryRef(value);
}
