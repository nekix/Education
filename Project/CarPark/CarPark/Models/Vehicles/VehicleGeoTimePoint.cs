using CarPark.Shared.DateTimes;
using NetTopologySuite.Geometries;

namespace CarPark.Models.Vehicles;

public sealed class VehicleGeoTimePoint
{
    public Guid Id { get; private set; }

    public Vehicle Vehicle { get; private set; }

    public Point Location { get; private set; }

    public UtcDateTimeOffset Time { get; private set; }

    #pragma warning disable CS8618  
    [Obsolete("Only for ORM and deserialization! Do not use!")]
    private VehicleGeoTimePoint()
    {
        // Only for ORM and deserialization! Do not use!
    }
    #pragma warning restore CS8618

    public VehicleGeoTimePoint(Guid id, Vehicle vehicle, Point location, UtcDateTimeOffset time)
    {
        Id = id;
        Vehicle = vehicle;
        Location = location;
        Time = time;
    }
}