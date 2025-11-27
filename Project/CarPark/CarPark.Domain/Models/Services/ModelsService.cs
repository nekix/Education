using CarPark.Models.Errors;
using CarPark.Vehicles;
using FluentResults;
using static CarPark.Models.Errors.ModelDomainErrorCodes;

namespace CarPark.Models.Services;

public class ModelsService : IModelsService
{
    public Result<Model> CreateModel(CreateModelRequest request)
    {
        ModelCreateData data = new ModelCreateData
        {
            Id = request.Id,
            ModelName = request.ModelName,
            VehicleType = request.VehicleType,
            SeatsCount = request.SeatsCount,
            MaxLoadingWeightKg = request.MaxLoadingWeightKg,
            EnginePowerKW = request.EnginePowerKW,
            TransmissionType = request.TransmissionType,
            FuelSystemType = request.FuelSystemType,
            FuelTankVolumeLiters = request.FuelTankVolumeLiters
        };

        return Model.Create(data);
    }

    public Result UpdateModel(Model model, UpdateModelRequest request)
    {
        ModelUpdateData data = new ModelUpdateData
        {
            ModelName = request.ModelName,
            VehicleType = request.VehicleType,
            SeatsCount = request.SeatsCount,
            MaxLoadingWeightKg = request.MaxLoadingWeightKg,
            EnginePowerKW = request.EnginePowerKW,
            TransmissionType = request.TransmissionType,
            FuelSystemType = request.FuelSystemType,
            FuelTankVolumeLiters = request.FuelTankVolumeLiters
        };

        return Model.Update(model, data);
    }

    public Result CheckCanDeleteModel(Model model, IEnumerable<Vehicle> modelVehicles)
    {
         if (modelVehicles.Any())
             return Result.Fail(new ModelDomainError(ModelHasVehiclesError));

         return Result.Ok();
    }
}