using System.Net.Http.Json;

namespace Intake.Application.Services;

public record AddressResult(string Street, string HouseNumber, string ZipCode, decimal Latitude, decimal Longitude);

public interface IAddressLookupClient
{
    Task<AddressResult?> GetAddress(string street, string houseNumber, string zipCode);
}

public class DawaClient : IAddressLookupClient
{
    public record DataWashResult(string Kategori, IEnumerable<DataWashMatch> Resultater);

    public record DataWashMatch(DataWashAddress AktuelAdresse);
    public record DataWashAddress(string Vejnavn, string Husnr, string Postnr, string Href);

    public record DataWashAddressInfo(DataWashAccessPoint Adgangspunkt);
    public record DataWashAccessPoint(decimal[] Koordinater);

    private readonly HttpClient _httpClient;

    public DawaClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AddressResult?> GetAddress(string street, string houseNumber, string zipCode)
    {
        var uri = new UriBuilder("https://api.dataforsyningen.dk/datavask/adgangsadresser")
        {
            Query = $"?betegnelse={street} {houseNumber}, {zipCode}"
        };

        try
        {
            var washResult = await _httpClient.GetFromJsonAsync<DataWashResult>(uri.Uri);

            if (washResult is not null)
            {
                var bestResult = washResult.Resultater?.FirstOrDefault();

                if (bestResult is not null)
                {
                    var idResult = await _httpClient.GetFromJsonAsync<DataWashAddressInfo>(bestResult.AktuelAdresse.Href);

                    if (idResult is not null)
                    {
                        return new AddressResult(bestResult.AktuelAdresse.Vejnavn, bestResult.AktuelAdresse.Husnr, bestResult.AktuelAdresse.Postnr, idResult.Adgangspunkt.Koordinater[0], idResult.Adgangspunkt.Koordinater[1]);
                    }
                }
            }
        }
        catch { }

        return null;
    }
}