namespace TreeCampaign.Domain.Stops;

public sealed record StopId(Guid Value)
{
    public static bool TryParse(string? input, out StopId stopId)
    {
        if (Guid.TryParse(input, out var guid))
        {
            stopId = new StopId(guid);
            return true;
        }

        stopId = new StopId(Guid.Empty);
        return false;
    }

    public static StopId From(Guid value) => new StopId(value);
};
