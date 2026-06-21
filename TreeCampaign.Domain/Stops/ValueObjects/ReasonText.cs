namespace TreeCampaign.Domain.Stops.ValueObjects;

public sealed record ReasonText(string Text)
{
    public static bool TryParse(string? input, out ReasonText reasonText)
    {
        if (!string.IsNullOrWhiteSpace(input))
        {
            reasonText = From(input);
            return true;
        }

        reasonText = Empty;
        return false;
    }

    public static ReasonText Empty = new ReasonText(string.Empty);

    public static ReasonText From(string text) => new ReasonText(text);
}
