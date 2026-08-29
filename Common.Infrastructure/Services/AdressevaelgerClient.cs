using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Common.Infrastructure.Services;

public class AdressevaelgerClient : IAddressLookupClient
{
    private const string Token = "adressevaelger123";
    private const int UtmZone = 32;

    public record SearchResult(string Status, string Beskrivelse, IEnumerable<SearchMatch> Fund);
    public record SearchMatch(string Id, string Vejnavn, string? Husnummer);

    public record StreetSearchResponse(string Status, string Beskrivelse, IEnumerable<StreetSearchMatch> Fund);
    public record StreetSearchMatch(string Id, string Vejnavn, string Postnr, string Postdistrikt, int AntalHusnumre);

    public record DetailResult(string Status, HusnummerDetail Husnummer);
    public record HusnummerDetail(string Husnummertekst, string Vejnavn, Postnummer Postnummer, Adgangspunkt Adgangspunkt);
    public record Postnummer(string Navn, string Postnr);
    public record Adgangspunkt(Geometri Geometri);
    public record Geometri(string Type, [property: JsonPropertyName("coordinates")] double[] Coordinates);

    private readonly HttpClient _httpClient;

    public AdressevaelgerClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AddressResult?> GetAddress(string street, string houseNumber, string zipCode)
    {
        try
        {
            var searchUri = new UriBuilder("https://adressevaelger.dk/husnumre/soeg/")
            {
                Query = $"?vejnavn={Uri.EscapeDataString(street)}&postnummer={Uri.EscapeDataString(zipCode)}&husnummer={Uri.EscapeDataString(houseNumber)}&token={Token}"
            };

            var searchResult = await _httpClient.GetFromJsonAsync<SearchResult>(searchUri.Uri);

            var match = searchResult?.Fund.FirstOrDefault(f =>
                string.Equals(f.Husnummer, houseNumber, StringComparison.OrdinalIgnoreCase));

            if (match is null)
                return null;

            var detailUri = new UriBuilder($"https://adressevaelger.dk/husnumre/{match.Id}/")
            {
                Query = $"?token={Token}"
            };

            var detail = await _httpClient.GetFromJsonAsync<DetailResult>(detailUri.Uri);
            var coordinates = detail?.Husnummer.Adgangspunkt.Geometri.Coordinates;

            if (coordinates is not { Length: 2 })
                return null;

            var (latitude, longitude) = UtmConverter.ToLatLon(coordinates[0], coordinates[1], UtmZone, isNorthern: true);

            return new AddressResult(
                detail!.Husnummer.Vejnavn,
                detail.Husnummer.Husnummertekst,
                detail.Husnummer.Postnummer.Postnr,
                latitude,
                longitude);
        }
        catch
        {
            return null;
        }
    }

    public async Task<IReadOnlyList<StreetSearchResult>> SearchStreets(string street, string zipCode)
    {
        try
        {
            var searchUri = new UriBuilder("https://adressevaelger.dk/soeg")
            {
                Query = $"?vejnavn={Uri.EscapeDataString(street)}&postnummer={Uri.EscapeDataString(zipCode)}&token={Token}&maal=navngivenvejpostnummer"
            };

            var searchResult = await _httpClient.GetFromJsonAsync<StreetSearchResponse>(searchUri.Uri);

            return searchResult?.Fund
                .Select(f => new StreetSearchResult(f.Vejnavn, f.Postnr, f.Postdistrikt, f.AntalHusnumre))
                .ToList()
                ?? [];
        }
        catch
        {
            return [];
        }
    }
}
