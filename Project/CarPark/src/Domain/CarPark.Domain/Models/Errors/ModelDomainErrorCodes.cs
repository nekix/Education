namespace CarPark.Models.Errors;

public static class ModelDomainErrorCodes
{
    public const string ModelNameRequired = "ModelNameRequired";
    public const string VehicleTypeRequired = "VehicleTypeRequired";
    public const string SeatsCountMustBePositive = "SeatsCountMustBePositive";
    public const string MaxLoadingWeightKgMustBePositive = "MaxLoadingWeightKgMustBePositive";
    public const string EnginePowerKWMustBePositive = "EnginePowerKWMustBePositive";
    public const string TransmissionTypeRequired = "TransmissionTypeRequired";
    public const string FuelSystemTypeRequired = "FuelSystemTypeRequired";
    public const string FuelTankVolumeLitersRequired = "FuelTankVolumeLitersRequired";

    public const string ModelHasVehiclesError = "ModelHasVehiclesError";
}