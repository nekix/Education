using FluentResults;
using static CarPark.Vehicles.Errors.VehicleDomainErrorCodes;
using CarPark.Vehicles.Errors;

namespace CarPark.Vehicles.Services;

public class VehiclesService : IVehiclesService
{
    public Result<Vehicle> CreateVehicle(CreateVehicleRequest request)
    {
        VehicleCreateData data = new VehicleCreateData
        {
            Id = request.Id,
            Model = request.Model,
            Enterprise = request.Enterprise,
            VinNumber = request.VinNumber,
            Price = request.Price,
            ManufactureYear = request.ManufactureYear,
            Mileage = request.Mileage,
            Color = request.Color,
            AssignedDrivers = request.AssignedDrivers,
            ActiveAssignedDriver = request.ActiveAssignedDriver,
            AddedToEnterpriseAt = request.AddedToEnterpriseAt
        };

        return Vehicle.Create(data);
    }

    public Result UpdateVehicle(Vehicle vehicle, UpdateVehicleRequest request)
    {
        VehicleUpdateData data = new VehicleUpdateData
        {
            Model = request.Model,
            Enterprise = request.Enterprise,
            VinNumber = request.VinNumber,
            Price = request.Price,
            ManufactureYear = request.ManufactureYear,
            Mileage = request.Mileage,
            Color = request.Color,
            AssignedDrivers = request.AssignedDrivers,
            ActiveAssignedDriver = request.ActiveAssignedDriver,
            AddedToEnterpriseAt = request.AddedToEnterpriseAt
        };

        return Vehicle.Update(vehicle, data);
    }

    public Result CheckCanDeleteVehicle(Vehicle vehicle)
    {
        if (vehicle.AssignedDrivers.Any())
            return Result.Fail(new VehicleDomainError(CannotDeleteVehicleWithAssignedDrivers));

        return Result.Ok();
    }
}