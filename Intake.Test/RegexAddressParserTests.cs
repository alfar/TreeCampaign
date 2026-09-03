using Intake.Domain.Orders.Services;
using Intake.Domain.Services;

namespace Intake.Test;

public class RegexAddressParserTests
{
    private readonly RegexAddressParser _parser = new();

    [Theory]
    [InlineData("Hjortekæret 24, Virklund", "Hjortekæret", "24")]
    [InlineData("Remstrupvej 39 i Sydbyen. 2 stk 🌲", "Remstrupvej", "39i")]
    [InlineData("Skovbakken 2 Gjessø", "Skovbakken", "2")]
    [InlineData("Hjortekæret 10, Virklund , 8600 Silkeborg", "Hjortekæret", "10")]
    [InlineData("Lyngbygade 49 1.th står ved vejen Lars Overgaard.", "Lyngbygade", "49")]
    [InlineData("Afh. Vejlbovej 46, 8600", "Vejlbovej", "46")]
    [InlineData("Almindsøvej 49, juletræ afhent", "Almindsøvej", "49")]
    [InlineData("Ellesvinget 6Virklund", "Ellesvinget", "6")]
    [InlineData("Rosenborgbakken 3, Virklund 8600", "Rosenborgbakken", "3")]
    [InlineData("Bryndumsvej 4, 2. sal - 1 stk. juletræ", "Bryndumsvej", "4")]
    [InlineData("Dalgasgade 44 8600", "Dalgasgade", "44")]
    [InlineData("Vestergade 45 a i baggården ved gårdhaven", "Vestergade", "45a")]
    [InlineData("Sanatorievej 18, 8600 Silkeborg - Kenneth & Laura", "Sanatorievej", "18")]
    [InlineData("2 stk - Ekkodalen 25", "Ekkodalen", "25")]
    [InlineData("Lyngbygade 87 - tusind tak 😀", "Lyngbygade", "87")]
    [InlineData("Vestervænget 3 😍", "Vestervænget", "3")]
    [InlineData("Søvænget 7. - 2 stk", "Søvænget", "7")]
    public void TryParse_RealWorldMessages_ExtractsStreetAndHouseNumber(
        string message, string expectedStreet, string expectedHouseNumber)
    {
        var result = _parser.TryParse(message);

        Assert.NotNull(result);
        Assert.Equal(expectedStreet, result!.Street);
        Assert.Equal(expectedHouseNumber, result.HouseNumber);
    }

    [Theory]
    [InlineData("Hjortekæret 10, Virklund , 8600 Silkeborg", "8600")]
    [InlineData("Afh. Vejlbovej 46, 8600", "8600")]
    [InlineData("Rosenborgbakken 3, Virklund 8600", "8600")]
    [InlineData("Dalgasgade 44 8600", "8600")]
    [InlineData("Sanatorievej 18, 8600 Silkeborg - Kenneth & Laura", "8600")]
    public void TryParse_MessagesWithZipCode_ExtractsZipCode(string message, string expectedZip)
    {
        var result = _parser.TryParse(message);

        Assert.NotNull(result);
        Assert.Equal(expectedZip, result!.ZipCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Tak for et flot arrangement!")]
    [InlineData("2 stk juletræer, mange tak")]
    public void TryParse_MessagesWithNoAddress_ReturnsNull(string message)
    {
        var result = _parser.TryParse(message);

        Assert.Null(result);
    }

    [Fact]
    public void TryParse_MessageWithoutZipCode_ReturnsNullZipCode()
    {
        var result = _parser.TryParse("Skovbakken 2 Gjessø");

        Assert.NotNull(result);
        Assert.Null(result!.ZipCode);
    }
}
