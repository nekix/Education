namespace CarPark.Services.GeoServices.GeoCodingServices;

public interface IGeoCodingService
{
    public Task<GeoDetails?> GetGeoDetailsAsync(GetGeoDatailsRequest request, CancellationToken token = default);
}