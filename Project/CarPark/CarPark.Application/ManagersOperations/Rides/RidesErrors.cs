using CarPark.Errors;
using CarPark.Rides.Errors;
using FluentResults;

namespace CarPark.ManagersOperations.Rides;

public static class RidesErrors
{
    public static Error MapDomainError(RideDomainError domainError)
    {
        return domainError.Code switch
        {
            RideDomainErrorCodes.StartTimeMustBeLessThanOrEqualToEndTime => new WebApiError(400, "Start time must be less than or equal to end time.").CausedBy(domainError),
            RideDomainErrorCodes.StartTimeMustBeGreaterThanOrEqualToStartPointTime => new WebApiError(400, "Start time must be greater than or equal to start point time.").CausedBy(domainError),
            RideDomainErrorCodes.EndTimeMustBeGreaterThanOrEqualToEndPointTime => new WebApiError(400, "End time must be greater than or equal to end point time.").CausedBy(domainError),
            RideDomainErrorCodes.StartPointVehicleMustMatchRideVehicle => new WebApiError(400, "Start point vehicle must match ride vehicle.").CausedBy(domainError),
            RideDomainErrorCodes.EndPointVehicleMustMatchRideVehicle => new WebApiError(400, "End point vehicle must match ride vehicle.").CausedBy(domainError),
            _ => new WebApiError(500, "An unexpected error occurred.").CausedBy(domainError)
        };
    }
}