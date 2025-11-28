using CarPark.Errors;
using CarPark.Vehicles.Errors;
using FluentResults;

namespace CarPark.ManagersOperations.Tracks;

internal static class TrackErrors
{
    internal static class VehicleGeoTimePoint
    {
        internal static Error MapDomainError(VehicleGeoTimePointDomainError domainError)
        {
            return domainError.Code switch
            {
                VehicleGeoTimePointDomainErrorCodes.VehicleMustBeDefined => new WebApiError(400, "Vehicle must be defined.").CausedBy(domainError),
                VehicleGeoTimePointDomainErrorCodes.LocationMustBeDefined => new WebApiError(400, "Location must be defined.").CausedBy(domainError),
                _ => new WebApiError(500, "An unexpected error occurred.").CausedBy(domainError)
            };
        }
    }
}