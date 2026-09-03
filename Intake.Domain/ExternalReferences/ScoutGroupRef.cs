namespace Intake.Domain.ExternalReferences;

public record ScoutGroupRef(Guid Value)
{
    public static ScoutGroupRef From(Guid value) => new ScoutGroupRef(value);
}
