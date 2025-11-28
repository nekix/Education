using CarPark.DateTimes;
using NetTopologySuite.Geometries;

namespace CarPark.Vehicles.Services;

public record UpdateVehicleGeoTimePointRequest
{
    public required Vehicle Vehicle { get; init; }
    public required Point Location { get; init; }
    public required UtcDateTimeOffset Time { get; init; }
}