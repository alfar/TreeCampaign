namespace TreeCampaign.Domain.Stops.ValueObjects;

public sealed record TreeCount(int Value)
{
    public static TreeCount From(int value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Tree count cannot be negative.");
        }

        return new TreeCount(value);
    }
}
