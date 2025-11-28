using CarPark.Models.Errors;
using FluentResults;
using static CarPark.Models.Errors.ModelDomainErrorCodes;

namespace CarPark.Models;

public sealed class Model
{
    public Guid Id { get; private set; }

    public string ModelName { get; private set; }

    public string VehicleType { get; private set; }

    public int SeatsCount { get; private set; }

    public double MaxLoadingWeightKg { get; private set; }

    public double EnginePowerKW { get; private set; }

    public string TransmissionType { get; private set; }

    public string FuelSystemType { get; private set; }

    // TODO: Заменить на double
    public string FuelTankVolumeLiters { get; private set; }

    #pragma warning disable CS8618
    [Obsolete("Only for ORM and deserialization! Do not use!")]
    private Model()
    {
        // Only for ORM and deserialization! Do not use!
    }
    #pragma warning restore CS8618

    private Model(
        Guid id, 
        string modelName,
        string vehicleType,
        int seatsCount,
        double maxLoadingWeightKg,
        double enginePowerKw,
        string transmissionType,
        string fuelSystemType,
        string fuelTankVolumeLiters)
    {
        Id = id;
        ModelName = modelName;
        VehicleType = vehicleType;
        SeatsCount = seatsCount;
        MaxLoadingWeightKg = maxLoadingWeightKg;
        EnginePowerKW = enginePowerKw;
        TransmissionType = transmissionType;
        FuelSystemType = fuelSystemType;
        FuelTankVolumeLiters = fuelTankVolumeLiters;
    }

    internal static Result<Model> Create(ModelCreateData data)
    {
        Result validationResult = ValidateModelName(data.ModelName)
            .Bind(() => ValidateVehicleType(data.VehicleType))
            .Bind(() => ValidateSeatsCount(data.SeatsCount))
            .Bind(() => ValidateMaxLoadingWeightKg(data.MaxLoadingWeightKg))
            .Bind(() => ValidateEnginePowerKW(data.EnginePowerKW))
            .Bind(() => ValidateTransmissionType(data.TransmissionType))
            .Bind(() => ValidateFuelSystemType(data.FuelSystemType))
            .Bind(() => ValidateFuelTankVolumeLiters(data.FuelTankVolumeLiters));

        if (validationResult.IsFailed)
            return Result.Fail<Model>(validationResult.Errors);

        Model model = new Model(
            data.Id,
            data.ModelName,
            data.VehicleType,
            data.SeatsCount,
            data.MaxLoadingWeightKg,
            data.EnginePowerKW,
            data.TransmissionType,
            data.FuelSystemType,
            data.FuelTankVolumeLiters);

        return Result.Ok(model);
    }

    internal static Result Update(Model model, ModelUpdateData data)
    {
        Result validationResult = ValidateModelName(data.ModelName)
            .Bind(() => ValidateVehicleType(data.VehicleType))
            .Bind(() => ValidateSeatsCount(data.SeatsCount))
            .Bind(() => ValidateMaxLoadingWeightKg(data.MaxLoadingWeightKg))
            .Bind(() => ValidateEnginePowerKW(data.EnginePowerKW))
            .Bind(() => ValidateTransmissionType(data.TransmissionType))
            .Bind(() => ValidateFuelSystemType(data.FuelSystemType))
            .Bind(() => ValidateFuelTankVolumeLiters(data.FuelTankVolumeLiters));

        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        model.ModelName = data.ModelName;
        model.VehicleType = data.VehicleType;
        model.SeatsCount = data.SeatsCount;
        model.MaxLoadingWeightKg = data.MaxLoadingWeightKg;
        model.EnginePowerKW = data.EnginePowerKW;
        model.TransmissionType = data.TransmissionType;
        model.FuelSystemType = data.FuelSystemType;
        model.FuelTankVolumeLiters = data.FuelTankVolumeLiters;

        return Result.Ok();
    }

    private static Result ValidateModelName(string modelName)
    {
        if (string.IsNullOrWhiteSpace(modelName))
            return Result.Fail(new ModelDomainError(ModelNameRequired));

        return Result.Ok();
    }

    private static Result ValidateVehicleType(string vehicleType)
    {
        if (string.IsNullOrWhiteSpace(vehicleType))
            return Result.Fail(new ModelDomainError(VehicleTypeRequired));

        return Result.Ok();
    }

    private static Result ValidateSeatsCount(int seatsCount)
    {
        if (seatsCount < 1)
            return Result.Fail(new ModelDomainError(SeatsCountMustBePositive));

        return Result.Ok();
    }

    private static Result ValidateMaxLoadingWeightKg(double maxLoadingWeightKg)
    {
        if (maxLoadingWeightKg <= 0)
            return Result.Fail(new ModelDomainError(MaxLoadingWeightKgMustBePositive));

        return Result.Ok();
    }

    private static Result ValidateEnginePowerKW(double enginePowerKW)
    {
        if (enginePowerKW <= 0)
            return Result.Fail(new ModelDomainError(EnginePowerKWMustBePositive));

        return Result.Ok();
    }

    private static Result ValidateTransmissionType(string transmissionType)
    {
        if (string.IsNullOrWhiteSpace(transmissionType))
            return Result.Fail(new ModelDomainError(TransmissionTypeRequired));

        return Result.Ok();
    }

    private static Result ValidateFuelSystemType(string fuelSystemType)
    {
        if (string.IsNullOrWhiteSpace(fuelSystemType))
            return Result.Fail(new ModelDomainError(FuelSystemTypeRequired));

        return Result.Ok();
    }

    private static Result ValidateFuelTankVolumeLiters(string fuelTankVolumeLiters)
    {
        if (string.IsNullOrWhiteSpace(fuelTankVolumeLiters))
            return Result.Fail(new ModelDomainError(FuelTankVolumeLitersRequired));

        return Result.Ok();
    }
}