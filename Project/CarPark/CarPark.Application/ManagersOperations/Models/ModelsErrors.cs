using CarPark.Errors;
using CarPark.Models.Errors;

namespace CarPark.ManagersOperations.Models;

public static class ModelsErrors
{
    public static WebApiError MapDomainError(ModelDomainError domainError)
    {
        return domainError.Code switch
        {
            ModelDomainErrorCodes.ModelNameRequired => new WebApiError(400, "Model name is required."),
            ModelDomainErrorCodes.VehicleTypeRequired => new WebApiError(400, "Vehicle type is required."),
            ModelDomainErrorCodes.SeatsCountMustBePositive => new WebApiError(400, "Seats count must be positive."),
            ModelDomainErrorCodes.MaxLoadingWeightKgMustBePositive => new WebApiError(400, "Max loading weight must be positive."),
            ModelDomainErrorCodes.EnginePowerKWMustBePositive => new WebApiError(400, "Engine power must be positive."),
            ModelDomainErrorCodes.TransmissionTypeRequired => new WebApiError(400, "Transmission type is required."),
            ModelDomainErrorCodes.FuelSystemTypeRequired => new WebApiError(400, "Fuel system type is required."),
            ModelDomainErrorCodes.FuelTankVolumeLitersRequired => new WebApiError(400, "Fuel tank volume is required."),
            ModelDomainErrorCodes.ModelHasVehiclesError => new WebApiError(409, "Cannot delete model because it is referenced by vehicles."),
            _ => new WebApiError(500, "An unexpected error occurred.")
        };
    }
}