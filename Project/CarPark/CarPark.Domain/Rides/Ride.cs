using CarPark.Vehicles;

namespace CarPark.Rides;

public class Ride
{
    public Guid Id { get; set; }

    public Vehicle Vehicle { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset EndTime { get; set; }

    public VehicleGeoTimePoint StartPoint { get; set; }

    public VehicleGeoTimePoint EndPoint { get; set; }
}