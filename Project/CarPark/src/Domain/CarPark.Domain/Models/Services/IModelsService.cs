using CarPark.Vehicles;
using FluentResults;

namespace CarPark.Models.Services;

public interface IModelsService
{
    Result<Model> CreateModel(CreateModelRequest request);

    Result UpdateModel(Model model, UpdateModelRequest request);

    Result CheckCanDeleteModel(Model model, IEnumerable<Vehicle> modelVehicles);
}