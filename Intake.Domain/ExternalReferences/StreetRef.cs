namespace Intake.Domain.ExternalReferences;

public record StreetRef(Guid Value)
{
    public static StreetRef From(Guid value) => new StreetRef(value);
}
