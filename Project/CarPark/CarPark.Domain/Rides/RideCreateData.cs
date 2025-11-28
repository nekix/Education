using CarPark.Vehicles;

namespace CarPark.Rides;

internal sealed record RideCreateData
{
    public required Guid Id { get; init; }

    public required Vehicle Vehicle { get; init; }

    public required DateTimeOffset StartTime { get; init; }

    public required DateTimeOffset EndTime { get; init; }

    public required VehicleGeoTimePoint StartPoint { get; init; }

    public required VehicleGeoTimePoint EndPoint { get; init; }
}