using FluentResults;

namespace CarPark.Vehicles.Services;

public interface IVehiclesService
{
    Result<Vehicle> CreateVehicle(CreateVehicleRequest request);
    Result UpdateVehicle(Vehicle vehicle, UpdateVehicleRequest request);
    Result CheckCanDeleteVehicle(Vehicle vehicle);
}