namespace Common.Infrastructure.Services;

public record AddressResult(string Street, string HouseNumber, string ZipCode, decimal Latitude, decimal Longitude);

public record StreetSearchResult(string StreetName, string ZipCode, string District, int HouseNumberCount);

public interface IAddressLookupClient
{
    Task<AddressResult?> GetAddress(string street, string houseNumber, string zipCode);
    Task<IReadOnlyList<StreetSearchResult>> SearchStreets(string street, string zipCode);
}
