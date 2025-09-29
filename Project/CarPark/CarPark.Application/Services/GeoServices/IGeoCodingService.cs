namespace CarPark.Services.GeoServices;

public interface IGeoCodingService
{
    public Task<GeoDetails?> GetGeoDetailsAsync(GetGeoDatailsRequest request, CancellationToken token = default);
}