namespace CarPark.Geo.GeoCoding;

public interface IGeoCodingService
{
    public Task<GeoDetails?> GetGeoDetailsAsync(GetGeoDatailsRequest request, CancellationToken token = default);
}