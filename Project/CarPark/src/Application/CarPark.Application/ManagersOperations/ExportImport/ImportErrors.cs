using CarPark.Errors;
using CarPark.Enterprises.Errors;
using CarPark.Models;
using CarPark.Models.Errors;
using CarPark.Rides.Errors;
using CarPark.Vehicles.Errors;
using FluentResults;

namespace CarPark.ManagersOperations.ExportImport;

internal static class ImportErrors
{
    internal static class Model
    {
        internal static Error MapDomainError(ModelDomainError domainError)
        {
            return domainError.Code switch
            {
                ModelDomainErrorCodes.ModelNameRequired => new WebApiError(400, "Model name is required.").CausedBy(domainError),
                ModelDomainErrorCodes.VehicleTypeRequired => new WebApiError(400, "Vehicle type is required.").CausedBy(domainError),
                ModelDomainErrorCodes.SeatsCountMustBePositive => new WebApiError(400, "Seats count must be positive.").CausedBy(domainError),
                ModelDomainErrorCodes.MaxLoadingWeightKgMustBePositive => new WebApiError(400, "Max loading weight must be positive.").CausedBy(domainError),
                ModelDomainErrorCodes.EnginePowerKWMustBePositive => new WebApiError(400, "Engine power must be positive.").CausedBy(domainError),
                ModelDomainErrorCodes.TransmissionTypeRequired => new WebApiError(400, "Transmission type is required.").CausedBy(domainError),
                ModelDomainErrorCodes.FuelSystemTypeRequired => new WebApiError(400, "Fuel system type is required.").CausedBy(domainError),
                ModelDomainErrorCodes.FuelTankVolumeLitersRequired => new WebApiError(400, "Fuel tank volume is required.").CausedBy(domainError),
                _ => new WebApiError(500, "An unexpected error occurred.").CausedBy(domainError)
            };
        }
    }

    internal static class Enterprise
    {
        internal static Error MapDomainError(EnterpriseDomainError domainError)
        {
            return domainError.Code switch
            {
                EnterpriseDomainErrorCodes.EnterpriseHasOtherManagersError => new WebApiError(409, "Cannot delete enterprise because it has assigned managers.").CausedBy(domainError),
                EnterpriseDomainErrorCodes.EnterpriseHasVehiclesError => new WebApiError(409, "Cannot delete enterprise because it has assigned vehicles.").CausedBy(domainError),
                EnterpriseDomainErrorCodes.EnterpriseHasDriversError => new WebApiError(409, "Cannot delete enterprise because it has assigned drivers.").CausedBy(domainError),
                _ => new WebApiError(500, "An unexpected error occurred.").CausedBy(domainError)
            };
        }
    }

    internal static class Vehicle
    {
        internal static Error MapDomainError(VehicleDomainError domainError)
        {
            return domainError.Code switch
            {
                VehicleDomainErrorCodes.VinNumberRequired => new WebApiError(400, "VIN number is required.").CausedBy(domainError),
                VehicleDomainErrorCodes.PriceMustBePositive => new WebApiError(400, "Price must be positive.").CausedBy(domainError),
                VehicleDomainErrorCodes.ManufactureYearMustBePositive => new WebApiError(400, "Manufacture year must be positive.").CausedBy(domainError),
                VehicleDomainErrorCodes.MileageMustBeNonNegative => new WebApiError(400, "Mileage must be non-negative.").CausedBy(domainError),
                VehicleDomainErrorCodes.ColorRequired => new WebApiError(400, "Color is required.").CausedBy(domainError),
                VehicleDomainErrorCodes.ModelMustBeDefined => new WebApiError(400, "Model must be defined.").CausedBy(domainError),
                VehicleDomainErrorCodes.EnterpriseMustBeDefined => new WebApiError(400, "Enterprise must be defined.").CausedBy(domainError),
                VehicleDomainErrorCodes.AssignedDriverFromAnotherEnterprise => new WebApiError(400, "Assigned driver is from another enterprise.").CausedBy(domainError),
                VehicleDomainErrorCodes.DuplicatedAssignedDriver => new WebApiError(400, "Duplicated assigned driver.").CausedBy(domainError),
                VehicleDomainErrorCodes.BeingRemovedAssignedDriverIsActive => new WebApiError(400, "Cannot remove active assigned driver.").CausedBy(domainError),
                VehicleDomainErrorCodes.ActiveAssignedDriverFromAnotherEnterprise => new WebApiError(400, "Active assigned driver is from another enterprise.").CausedBy(domainError),
                VehicleDomainErrorCodes.ActiveAssignedDriverMustBeInAssignedDrivers => new WebApiError(400, "Active assigned driver must be in assigned drivers.").CausedBy(domainError),
                VehicleDomainErrorCodes.ForbidChangeEnterpriseWhenExistAssignedDrivers => new WebApiError(400, "Cannot change enterprise when assigned drivers exist.").CausedBy(domainError),
                VehicleDomainErrorCodes.CannotDeleteVehicleWithAssignedDrivers => new WebApiError(409, "Cannot delete vehicle with assigned drivers.").CausedBy(domainError),
                _ => new WebApiError(500, "An unexpected error occurred.").CausedBy(domainError)
            };
        }
    }

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

    internal static class Ride
    {
        internal static Error MapDomainError(RideDomainError domainError)
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
}