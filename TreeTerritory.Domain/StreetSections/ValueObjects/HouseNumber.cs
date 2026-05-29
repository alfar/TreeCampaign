public record HouseNumber(int Number, char? Letter = null) : IComparable<HouseNumber>
{
    public static HouseNumber Parse(string value)
    {
        if (TryParse(value, out var houseNumber))
        {
            return houseNumber;
        }

        throw new FormatException($"Invalid house number format: '{value}'.");
    }

    public static bool TryParse(string? input, out HouseNumber houseNumber)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            houseNumber = default!;
            return false;
        }

        if (!char.IsDigit(input[0]))

        {
            houseNumber = default!;
            return false;
        }

        var numberPart = new string([.. input.TakeWhile(char.IsDigit)]);
        var letterPart = input.Length > numberPart.Length ? input[numberPart.Length] : default(char?);
        houseNumber = new HouseNumber(int.Parse(numberPart), letterPart);
        return true;
    }

    public int CompareTo(HouseNumber? other)
    {
        if (other is null) return 1;
        var numberComparison = Number.CompareTo(other.Number);
        if (numberComparison != 0) return numberComparison;
        return Nullable.Compare(Letter, other.Letter);
    }

    public override string ToString() => Letter.HasValue ? $"{Number}{Letter}" : Number.ToString();
}
