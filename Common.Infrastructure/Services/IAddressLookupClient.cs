namespace Common.Infrastructure.Services;

public abstract record AddressResult;

public sealed record SuccessfulAddressResult(
    string Street, string HouseNumber, string ZipCode, decimal Latitude, decimal Longitude) : AddressResult;

public sealed record FailedAddressResult(string Reason) : AddressResult;

public record StreetSearchResult(string StreetName, string ZipCode, string District, int HouseNumberCount);

public interface IAddressLookupClient
{
    Task<AddressResult> GetAddress(string street, string houseNumber, string zipCode);
    Task<IReadOnlyList<StreetSearchResult>> SearchStreets(string street, string zipCode);
}
