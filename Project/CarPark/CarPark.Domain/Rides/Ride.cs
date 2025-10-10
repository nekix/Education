using CarPark.Vehicles;

namespace CarPark.Rides;

public class Ride
{
    public required Guid Id { get; set; }

    public required Vehicle Vehicle { get; set; }

    public required DateTimeOffset StartTime { get; set; }

    public required DateTimeOffset EndTime { get; set; }

    public required VehicleGeoTimePoint StartPoint { get; set; }

    public required VehicleGeoTimePoint EndPoint { get; set; }
}