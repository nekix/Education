using CarPark.Shared.DateTimes;
using FluentResults;
using NetTopologySuite.Geometries;

namespace CarPark.Vehicles;

public sealed class VehicleGeoTimePoint
{
    public Guid Id { get; private init; }

    public Vehicle Vehicle { get; private set; }

    public Point Location { get; private set; }

    public UtcDateTimeOffset Time { get; private set; }

    #pragma warning disable CS8618  
    [Obsolete("Only for ORM, factrory method and deserialization! Do not use!")]
    private VehicleGeoTimePoint()
    {
        // Only for ORM and deserialization! Do not use!
    }
    #pragma warning restore CS8618

    public static Result<VehicleGeoTimePoint> Create(Guid id, Vehicle vehicle, Point location, UtcDateTimeOffset time)
    {
        #pragma warning disable CS0618
        VehicleGeoTimePoint point = new VehicleGeoTimePoint
        {
            Id = id,
        };
        #pragma warning restore CS0618

        if (vehicle == null)
        {
            return Result.Fail<VehicleGeoTimePoint>(VehiclesGeoTimePointErrors.VehicleMustBeDefined);
        }
        point.Vehicle = vehicle;

        if (location == null)
        {
            return Result.Fail<VehicleGeoTimePoint>(VehiclesGeoTimePointErrors.LocationMustBeDefined);
        }
        point.Location = location;

        point.Time = time;

        return Result.Ok(point);
    }
}