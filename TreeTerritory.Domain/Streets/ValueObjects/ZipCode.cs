namespace TreeTerritory.Domain.Streets.ValueObjects;

public record ZipCode(string Value)
{
    public static ZipCode Empty => new ZipCode(string.Empty);

    public static bool TryParse(string? input, out ZipCode zipCode)
    {
        if (string.IsNullOrEmpty(input) || input.Any(c => !char.IsDigit(c)))
        {
            zipCode = default!;
            return false;
        }

        zipCode = From(input);
        return true;
    }

    public static ZipCode From(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length != 4 || value.Any(c => !char.IsDigit(c)))
        {
            throw new ArgumentException("Invalid zip code format.", nameof(value));
        }

        return new ZipCode(value);
    }

    public override string ToString() => Value;
}