using CarPark.Data;
using CarPark.Drivers;
using CarPark.Enterprises;
using CarPark.Managers;
using CarPark.Models;
using CarPark.Shared.CQ;
using CarPark.Vehicles;
using FluentResults;
using FluentResults.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CarPark.ManagersOperations.Vehicles.Commands;

internal class ManagersVehiclesCommandHandler : BaseManagersHandler,
    ICommandHandler<CreateVehicleCommand, Result<int>>, 
    ICommandHandler<UpdateVehicleCommand, Result<int>>,
    ICommandHandler<DeleteVehicleCommand, Result>
{
    public ManagersVehiclesCommandHandler(ApplicationDbContext dbContext) : base(dbContext)
    {

    }

    public async Task<Result<int>> Handle(CreateVehicleCommand command)
    {
        Result<Manager> getManager = await GetManagerAsync(command.RequestingManagerId, false);
        Result<Enterprise> getEnterprise = getManager.Bind(manager => GetAllowedToManagerEnterprise(manager, command.EnterpriseId));
        Result<Model> getModel = await getEnterprise.Bind(_ => GetModelAsync(command.ModelId));
        Result<List<Driver>> getAssignedDrivers = await getModel.Bind(_ => GetAssignedDriversAsync(command.DriverIds));
        Result<Driver?> getIfDefinedActiveAssignedDriver = await getAssignedDrivers.Bind(_ => GetIfDefinedActiveAssignedDriverAsync(command.ActiveDriverId));

        Result<Vehicle> createVehicle = getIfDefinedActiveAssignedDriver
            .Bind(activeAssignedDriver => 
                Vehicle.Create(
                    default, 
                    getModel.Value, 
                    getEnterprise.Value, 
                    command.VinNumber, 
                    command.Price, 
                    command.ManufactureYear,
                    command.Mileage, 
                    command.Color, 
                    getAssignedDrivers.Value,
                    activeAssignedDriver,
                    command.AddedToEnterpriseAt));

        Result<Vehicle> saveNewVehicle = await createVehicle.Bind(SaveNewVehicleAsync);

        return saveNewVehicle.Map(v => v.Id);
    }

    public async Task<Result<int>> Handle(UpdateVehicleCommand command)
    {
        Result<Manager> getManager = await GetManagerAsync(command.RequestingManagerId, false);
        Result<Vehicle> getVehicle = await getManager.Bind(_ => GetVehicleAsync(command.VehicleId));
        Result<Enterprise> getOldEnterprise = getManager.Bind(m => GetAllowedToManagerEnterprise(m, getVehicle.Value.Enterprise.Id));
        if (getOldEnterprise.IsFailed)
            return getOldEnterprise.ToResult<int>();

        Vehicle vehicle = getVehicle.Value;

        if (vehicle.Enterprise.Id != command.EnterpriseId)
        {
            Result<Vehicle> setEnterprise = GetAllowedToManagerEnterprise(getManager.Value, command.EnterpriseId)
                .Bind(e => vehicle.SetEnterprise(e));
            if (setEnterprise.IsFailed)
                return setEnterprise.ToResult<int>();
        }

        if (vehicle.Model.Id != command.ModelId)
        {
            Result<Vehicle> setModel = await GetModelAsync(command.ModelId)
                .Bind(m => vehicle.SetModel(m));
            if (setModel.IsFailed)
                return setModel.ToResult<int>();
        }

        if (vehicle.ActiveAssignedDriver?.Id != command.ActiveDriverId
            || vehicle.AssignedDrivers.Count != command.DriverIds.Count
            || vehicle.AssignedDrivers.IntersectBy(command.DriverIds, v => v.Id).Count() !=
            vehicle.AssignedDrivers.Count)
        {
            Result<Vehicle> setDrivers = await SetDriversToVehicleAsync(vehicle, command.DriverIds, command.ActiveDriverId);
            if (setDrivers.IsFailed)
                return setDrivers.ToResult<int>();
        }

        if (vehicle.VinNumber != command.VinNumber)
        {
            vehicle.VinNumber = command.VinNumber;
        }

        if (vehicle.Price != command.Price)
        {
            vehicle.Price = command.Price;
        }

        if (vehicle.ManufactureYear != command.ManufactureYear)
        {
            vehicle.ManufactureYear = command.ManufactureYear;
        }

        if (vehicle.Mileage != command.Mileage)
        {
            vehicle.Mileage = command.Mileage;
        }

        if (vehicle.Color != command.Color)
        {
            vehicle.Color = command.Color;
        }

        if (vehicle.AddedToEnterpriseAt != command.AddedToEnterpriseAt)
        {
            vehicle.AddedToEnterpriseAt = command.AddedToEnterpriseAt;
        }

        Result<Vehicle> saveChanges = await SaveVehicleChangesAsync(vehicle);

        return saveChanges.Map(v => v.Id);
    }

    public async Task<Result> Handle(DeleteVehicleCommand command)
    {
        Result<Manager> getManager = await GetManagerAsync(command.RequestingManagerId, false);
        Result<Vehicle> getVehicle = await getManager.Bind(_ => GetVehicleAsync(command.VehicleId));
        Result<Enterprise> getEnterprise = getVehicle.Bind(manager => GetAllowedToManagerEnterprise(getManager.Value, command.VehicleId));
        if (getEnterprise.IsFailed)
            return getEnterprise.ToResult();

        Vehicle vehicle = getVehicle.Value;

        if (vehicle.AssignedDrivers.Count > 0)
            return Result.Fail(VehiclesHandlersErrors.ForbidDeleteVehicleWithAssignedDrivers);

        Result removeVehicle = await RemoveVehicleAsync(vehicle);

        return removeVehicle;
    }

    private async Task<Result<Vehicle>> GetVehicleAsync(int vehicleId)
    {
        Vehicle? vehicle = await DbContext.Vehicles
            .Include(v => v.Enterprise)
            .Include(v => v.Model)
            .Include(v => v.AssignedDrivers)
            .Include(v => v.ActiveAssignedDriver)
            .FirstOrDefaultAsync(v => v.Id == vehicleId);

        return vehicle != null
            ? Result.Ok(vehicle)
            : Result.Fail<Vehicle>(VehiclesHandlersErrors.VehicleNotExist);
    }

    private static Result<Enterprise> GetAllowedToManagerEnterprise(Manager manager, int enterpriseId)
    {
        Enterprise? enterprise = manager.Enterprises.FirstOrDefault(e => e.Id == enterpriseId);

        return enterprise != null
            ? Result.Ok(enterprise)
            : Result.Fail<Enterprise>(VehiclesHandlersErrors.ManagerNotAllowedToEnterprise);
    }

    private async Task<Result<Model>> GetModelAsync(int modelId)
    {
        Model? model = await DbContext.Models.FirstOrDefaultAsync(m => m.Id == modelId);

        return model != null
            ? Result.Ok(model)
            : Result.Fail<Model>(VehiclesHandlersErrors.ModelNotExist);
    }

    private async Task<Result<List<Driver>>> GetAssignedDriversAsync(List<int> assignedDriverIds)
    {
        List<Driver> assignedDrivers = await DbContext.Drivers
            .Where(d => assignedDriverIds.Contains(d.Id))
            .ToListAsync();

        return assignedDrivers.Count == assignedDriverIds.Count
            ? Result.Ok(assignedDrivers)
            : Result.Fail<List<Driver>>(VehiclesHandlersErrors.AssignedDriversNotExist);
    }

    private async Task<Result<Driver?>> GetIfDefinedActiveAssignedDriverAsync(int? activeAssignedDriverId)
    {
        if (activeAssignedDriverId == null)
            return Result.Ok<Driver?>(null);

        Driver? driver = await DbContext.Drivers
            .FirstOrDefaultAsync(d => d.Id == activeAssignedDriverId);

        return driver != null
            ? Result.Ok<Driver?>(driver)
            : Result.Fail<Driver?>(VehiclesHandlersErrors.ActiveAssignedDriversNotExist);
    }

    private async Task<Result<Vehicle>> SaveNewVehicleAsync(Vehicle vehicle)
    {
        await DbContext.Vehicles.AddAsync(vehicle);
        await DbContext.SaveChangesAsync();

        return Result.Ok(vehicle);
    }

    private async Task<Result<Vehicle>> SetDriversToVehicleAsync(Vehicle vehicle, List<int> assidnedDriversIds, int? activeAssignedDriverId)
    {
        Result<List<Driver>> getAssignedDrivers = await GetAssignedDriversAsync(assidnedDriversIds);
        Result<Driver?> getIfDefinedActiveAssignedDriver = await getAssignedDrivers.Bind(_ => GetIfDefinedActiveAssignedDriverAsync(activeAssignedDriverId));
        if (getIfDefinedActiveAssignedDriver.IsFailed)
            return getIfDefinedActiveAssignedDriver.ToResult<Vehicle>();

        foreach (Driver driver in getAssignedDrivers.Value)
        {
            if (vehicle.AssignedDrivers.All(d => d.Id != driver.Id))
            {
                Result<Vehicle> addDriver = vehicle.AddAssignedDriver(driver);
                if (addDriver.IsFailed)
                    return addDriver;
            }
        }

        Result<Vehicle> setActiveAssignedDriver = vehicle.SetActiveAssignedDriver(getIfDefinedActiveAssignedDriver.Value);
        if (setActiveAssignedDriver.IsFailed)
            return setActiveAssignedDriver;

        foreach (Driver driver in vehicle.AssignedDrivers)
        {
            if (getAssignedDrivers.Value.All(d => d.Id != driver.Id))
            {
                Result<Vehicle> deleteDriver = vehicle.DeleteAssignedDriver(driver);
                if (deleteDriver.IsFailed)
                    return deleteDriver;
            }
        }

        return Result.Ok(vehicle);
    }

    private async Task<Result<Vehicle>> SaveVehicleChangesAsync(Vehicle vehicle)
    {
        DbContext.Vehicles.Update(vehicle);
        await DbContext.SaveChangesAsync();

        return Result.Ok(vehicle);
    }

    private async Task<Result> RemoveVehicleAsync(Vehicle vehicle)
    {
        DbContext.Vehicles.Remove(vehicle);
        await DbContext.SaveChangesAsync();

        return Result.Ok();
    }
}