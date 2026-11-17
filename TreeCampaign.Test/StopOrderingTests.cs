using TreeCampaign.Domain.ExternalReferences;
using TreeCampaign.Domain.Stops;
using TreeCampaign.Domain.Stops.ValueObjects;

namespace TreeCampaign.Test;

public class StopOrderingTests
{
    private static readonly Guid SectionA = Guid.NewGuid();
    private static readonly Guid SectionB = Guid.NewGuid();
    private static readonly Guid Unknown = Guid.NewGuid();

    private static readonly Dictionary<Guid, SectionOrder> Sections = new()
    {
        [SectionA] = new SectionOrder(1, Descending: false),
        [SectionB] = new SectionOrder(2, Descending: true),
    };

    private static Address Addr(string name, Guid section) =>
        new(name, 0m, 0m, StreetSectionRef.From(section));

    private static string[] Sort(params Address[] addresses) =>
        addresses.OrderBy(a => a, StopOrdering.Comparer(Sections)).Select(a => a.DisplayName).ToArray();

    [Fact]
    public void Orders_by_section_sort_order_first()
    {
        var result = Sort(Addr("B 1, 8600", SectionB), Addr("A 9, 8600", SectionA));
        Assert.Equal(["A 9, 8600", "B 1, 8600"], result);
    }

    [Fact]
    public void Ascending_section_sorts_house_numbers_ascending()
    {
        var result = Sort(Addr("A 10, 8600", SectionA), Addr("A 2, 8600", SectionA));
        Assert.Equal(["A 2, 8600", "A 10, 8600"], result);
    }

    [Fact]
    public void Descending_section_sorts_house_numbers_descending()
    {
        var result = Sort(Addr("B 2, 8600", SectionB), Addr("B 10, 8600", SectionB));
        Assert.Equal(["B 10, 8600", "B 2, 8600"], result);
    }

    [Fact]
    public void Unknown_section_sorts_last()
    {
        var result = Sort(Addr("X 1, 8600", Unknown), Addr("B 1, 8600", SectionB));
        Assert.Equal(["B 1, 8600", "X 1, 8600"], result);
    }

    [Theory]
    [InlineData("Storgade 12, 8600 Silkeborg", 12)]
    [InlineData("Storgade 12B, 8600 Silkeborg", 12)]
    [InlineData("Storgade, 8600 Silkeborg", 0)]
    public void ParseHouseNumber_reads_leading_digits(string displayName, int expected) =>
        Assert.Equal(expected, StopOrdering.ParseHouseNumber(displayName));
}
