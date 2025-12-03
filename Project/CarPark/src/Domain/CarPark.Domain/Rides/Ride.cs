using CarPark.DateTimes;
using CarPark.Vehicles;
using CarPark.Rides.Errors;
using FluentResults;
using static CarPark.Rides.Errors.RideDomainErrorCodes;

namespace CarPark.Rides;

public sealed class Ride
{
    public Guid Id { get; private init; }

    public Vehicle Vehicle { get; private set; }

    public UtcDateTimeOffset StartTime { get; private set; }

    public UtcDateTimeOffset EndTime { get; private set; }

    public VehicleGeoTimePoint StartPoint { get; private set; }

    public VehicleGeoTimePoint EndPoint { get; private set; }

    #pragma warning disable CS8618
    [Obsolete("Only for ORM and deserialization! Do not use!")]
    private Ride()
    {
        // Only for ORM and deserialization! Do not use!
    }
    #pragma warning restore CS8618

    private Ride(
        Guid id,
        Vehicle vehicle,
        UtcDateTimeOffset startTime,
        UtcDateTimeOffset endTime,
        VehicleGeoTimePoint startPoint,
        VehicleGeoTimePoint endPoint)
    {
        Id = id;
        Vehicle = vehicle;
        StartTime = startTime;
        EndTime = endTime;
        StartPoint = startPoint;
        EndPoint = endPoint;
    }

    internal static Result<Ride> Create(RideCreateData data)
    {
        Result validationResult = ValidateStartTime(data.StartTime, data.EndTime);

        if (validationResult.IsFailed)
            return Result.Fail<Ride>(validationResult.Errors);

        #pragma warning disable CS0618
        Ride ride = new Ride(
            data.Id,
            data.Vehicle,
            data.StartTime,
            data.EndTime,
            data.StartPoint,
            data.EndPoint);
        #pragma warning restore CS0618

        return Result.Ok(ride);
    }

    private static Result ValidateStartTime(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        if (startTime > endTime)
            return Result.Fail(new RideDomainError(StartTimeMustBeLessThanOrEqualToEndTime));

        return Result.Ok();
    }
}