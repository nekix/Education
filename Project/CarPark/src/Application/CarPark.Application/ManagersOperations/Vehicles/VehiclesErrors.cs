using CarPark.Errors;
using CarPark.Vehicles.Errors;
using FluentResults;

namespace CarPark.ManagersOperations.Vehicles;

public static class VehiclesErrors
{
    public static Error MapDomainError(VehicleDomainError domainError)
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