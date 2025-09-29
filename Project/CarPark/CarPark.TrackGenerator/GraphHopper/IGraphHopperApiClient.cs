using CarPark.TrackGenerator.GraphHopper.Models;

namespace CarPark.TrackGenerator.GraphHopper;

public interface IGraphHopperApiClient
{
    Task<RouteResponse> GetRouteAsync(RouteRequest request);
}

