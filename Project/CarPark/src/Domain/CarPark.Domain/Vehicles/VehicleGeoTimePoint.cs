using CarPark.DateTimes;
using FluentResults;
using NetTopologySuite.Geometries;
using CarPark.Vehicles.Errors;
using static CarPark.Vehicles.Errors.VehicleGeoTimePointDomainErrorCodes;

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

    internal static Result<VehicleGeoTimePoint> Create(VehicleGeoTimePointCreateData data)
    {
        Result validationResult = ValidateVehicle(data.Vehicle)
            .Bind(() => ValidateLocation(data.Location));

        if (validationResult.IsFailed)
            return Result.Fail<VehicleGeoTimePoint>(validationResult.Errors);

        #pragma warning disable CS0618
        VehicleGeoTimePoint point = new VehicleGeoTimePoint
        {
            Id = data.Id,
            Vehicle = data.Vehicle,
            Location = data.Location,
            Time = data.Time
        };
        #pragma warning restore CS0618

        return Result.Ok(point);
    }

    internal static Result Update(VehicleGeoTimePoint point, VehicleGeoTimePointUpdateData data)
    {
        Result validationResult = ValidateVehicle(data.Vehicle)
            .Bind(() => ValidateLocation(data.Location));

        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        point.Vehicle = data.Vehicle;
        point.Location = data.Location;
        point.Time = data.Time;

        return Result.Ok();
    }

    private static Result ValidateVehicle(Vehicle vehicle)
    {
        if (vehicle == null)
            return Result.Fail(new VehicleGeoTimePointDomainError(VehicleMustBeDefined));

        return Result.Ok();
    }

    private static Result ValidateLocation(Point location)
    {
        if (location == null)
            return Result.Fail(new VehicleGeoTimePointDomainError(LocationMustBeDefined));

        return Result.Ok();
    }
}