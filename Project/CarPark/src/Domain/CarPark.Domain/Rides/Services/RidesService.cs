using CarPark.Rides.Errors;
using FluentResults;
using static CarPark.Rides.Errors.RideDomainErrorCodes;

namespace CarPark.Rides.Services;

public sealed class RidesService : IRidesService
{
    public Result<Ride> CreateRide(CreateRideRequest request)
    {
        // Validate that ride times are consistent with point times
        if (request.StartTime < request.StartPoint.Time)
        {
            return Result.Fail(new RideDomainError(StartTimeMustBeGreaterThanOrEqualToStartPointTime));
        }

        if (request.EndTime < request.EndPoint.Time)
        {
            return Result.Fail(new RideDomainError(EndTimeMustBeGreaterThanOrEqualToEndPointTime));
        }

        // Validate that points belong to the same vehicle as the ride
        if (request.StartPoint.Vehicle.Id != request.Vehicle.Id)
        {
            return Result.Fail(new RideDomainError(StartPointVehicleMustMatchRideVehicle));
        }

        if (request.EndPoint.Vehicle.Id != request.Vehicle.Id)
        {
            return Result.Fail(new RideDomainError(EndPointVehicleMustMatchRideVehicle));
        }

        RideCreateData data = new RideCreateData
        {
            Id = request.Id,
            Vehicle = request.Vehicle,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            StartPoint = request.StartPoint,
            EndPoint = request.EndPoint
        };

        return Ride.Create(data);
    }
}