using CarPark.Errors;
using CarPark.Models.Errors;
using FluentResults;

namespace CarPark.ManagersOperations.Models;

public static class ModelsErrors
{
    public static Error MapDomainError(ModelDomainError domainError)
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
            ModelDomainErrorCodes.ModelHasVehiclesError => new WebApiError(409, "Cannot delete model because it is referenced by vehicles.").CausedBy(domainError),
            _ => new WebApiError(500, "An unexpected error occurred.").CausedBy(domainError)
        };
    }
}