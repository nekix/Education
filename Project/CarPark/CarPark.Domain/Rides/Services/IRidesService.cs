using FluentResults;

namespace CarPark.Rides.Services;

public interface IRidesService
{
    Result<Ride> CreateRide(CreateRideRequest request);
}