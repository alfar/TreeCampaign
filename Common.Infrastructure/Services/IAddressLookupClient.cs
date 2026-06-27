namespace Common.Infrastructure.Services;

public record AddressResult(string Street, string HouseNumber, string ZipCode, decimal Latitude, decimal Longitude);

public interface IAddressLookupClient
{
    Task<AddressResult?> GetAddress(string street, string houseNumber, string zipCode);
}
