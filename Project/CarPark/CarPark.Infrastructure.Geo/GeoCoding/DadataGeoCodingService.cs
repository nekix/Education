using Dadata;
using Dadata.Model;
using Microsoft.Extensions.Options;

namespace CarPark.Geo.GeoCoding;

public class DadataGeoCodingService : IGeoCodingService
{
    private readonly SuggestClientAsync _cleanClient;

    public DadataGeoCodingService(IOptions<DadataSettings> settingsOptions,
        HttpClient httpClient)
    {
        DadataSettings settings = settingsOptions.Value;

        _cleanClient = new SuggestClientAsync(settings.Token,
            settings.Secret,
            settings.BaseUrl,
            httpClient);
    }

    public async Task<GeoDetails?> GetGeoDetailsAsync(GetGeoDatailsRequest request, CancellationToken token = default)
    {
        SuggestResponse<Address> response = await _cleanClient.Geolocate(request.Latitude, request.Longitude, 150000, 3, token);

        Suggestion<Address>? responseAddress = response.suggestions.FirstOrDefault();

        if (responseAddress == null)
            return null;

        return new GeoDetails
        {
            Address = responseAddress.value,
            Latitude = responseAddress.data.geo_lat,
            Longitude = responseAddress.data.geo_lon
        };
    }
}