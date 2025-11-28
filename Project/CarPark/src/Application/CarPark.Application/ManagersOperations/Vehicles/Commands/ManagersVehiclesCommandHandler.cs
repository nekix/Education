using CarPark.CQ;
using CarPark.Data;
using CarPark.Drivers;
using CarPark.Enterprises;
using CarPark.Errors;
using CarPark.Managers;
using CarPark.Models;
using CarPark.Vehicles;
using CarPark.Vehicles.Errors;
using CarPark.Vehicles.Services;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CarPark.ManagersOperations.Vehicles.Commands;

internal class ManagersVehiclesCommandHandler : BaseManagersHandler,
    ICommandHandler<CreateVehicleCommand, Result<Guid>>,
    ICommandHandler<UpdateVehicleCommand, Result>,
    ICommandHandler<DeleteVehicleCommand, Result>
{
    private readonly IVehiclesService _vehiclesService;

    public ManagersVehiclesCommandHandler(ApplicationDbContext dbContext, IVehiclesService vehiclesService) : base(dbContext)
    {
        _vehiclesService = vehiclesService;
    }

    public async Task<Result<Guid>> Handle(CreateVehicleCommand command)
    {
        Result<Manager> getManager = await GetManagerAsync(command.RequestingManagerId, false);
        Result<Enterprise> getEnterprise = getManager.Bind(manager => GetAllowedToManagerEnterprise(manager, command.EnterpriseId));
        Result<Model> getModel = await getEnterprise.Bind(_ => GetModelAsync(command.ModelId));
        Result<List<Driver>> getAssignedDrivers = await getModel.Bind(_ => GetAssignedDriversAsync(command.DriverIds));
        Result<Driver?> getIfDefinedActiveAssignedDriver = await getAssignedDrivers.Bind(_ => GetIfDefinedActiveAssignedDriverAsync(command.ActiveDriverId));
        if (getIfDefinedActiveAssignedDriver.IsFailed)
            return Result.Fail<Guid>(getIfDefinedActiveAssignedDriver.Errors);


        CreateVehicleRequest request = new CreateVehicleRequest
        {
            Id = default,
            Model = getModel.Value,
            Enterprise = getEnterprise.Value,
            VinNumber = command.VinNumber,
            Price = command.Price,
            ManufactureYear = command.ManufactureYear,
            Mileage = command.Mileage,
            Color = command.Color,
            AssignedDrivers = getAssignedDrivers.Value,
            ActiveAssignedDriver = getIfDefinedActiveAssignedDriver.Value,
            AddedToEnterpriseAt = command.AddedToEnterpriseAt
        };

        Result<Vehicle> createVehicle = _vehiclesService.CreateVehicle(request);
        if (createVehicle.IsFailed)
        {
            IEnumerable<IError> errors = createVehicle.Errors
                .Select(e => e is VehicleDomainError domainError ? VehiclesErrors.MapDomainError(domainError) : e);

            return Result.Fail<Guid>(errors);
        }

        Result<Vehicle> saveNewVehicle = await createVehicle.Bind(SaveNewVehicleAsync);

        return saveNewVehicle.Map(v => v.Id);
    }

    public async Task<Result> Handle(UpdateVehicleCommand command)
    {
        Result<Manager> getManager = await GetManagerAsync(command.RequestingManagerId, false);
        Result<Vehicle> getVehicle = await getManager.Bind(_ => GetVehicleAsync(command.VehicleId));
        Result<Enterprise> getOldEnterprise = getManager.Bind(m => GetAllowedToManagerEnterprise(m, getVehicle.Value.Enterprise.Id));
        if (getOldEnterprise.IsFailed)
            return getOldEnterprise.ToResult();

        Vehicle vehicle = getVehicle.Value;

        // Load new entities
        Result<Model> getModel = await GetModelAsync(command.ModelId);
        if (getModel.IsFailed) return getModel.ToResult();

        Result<Enterprise> getEnterprise = GetAllowedToManagerEnterprise(getManager.Value, command.EnterpriseId);
        if (getEnterprise.IsFailed) return getEnterprise.ToResult();

        Result<List<Driver>> getAssignedDrivers = await GetAssignedDriversAsync(command.DriverIds);
        if (getAssignedDrivers.IsFailed) return getAssignedDrivers.ToResult();

        Result<Driver?> getActiveDriver = await GetIfDefinedActiveAssignedDriverAsync(command.ActiveDriverId);
        if (getActiveDriver.IsFailed) return getActiveDriver.ToResult();

        UpdateVehicleRequest request = new UpdateVehicleRequest
        {
            Model = getModel.Value,
            Enterprise = getEnterprise.Value,
            VinNumber = command.VinNumber,
            Price = command.Price,
            ManufactureYear = command.ManufactureYear,
            Mileage = command.Mileage,
            Color = command.Color,
            AssignedDrivers = getAssignedDrivers.Value,
            ActiveAssignedDriver = getActiveDriver.Value,
            AddedToEnterpriseAt = command.AddedToEnterpriseAt
        };

        Result updateResult = _vehiclesService.UpdateVehicle(vehicle, request);
        if (updateResult.IsFailed)
        {
            IEnumerable<IError> errors = updateResult.Errors
                .Select(e => e is VehicleDomainError domainError ? VehiclesErrors.MapDomainError(domainError) : e);

            return Result.Fail(errors);
        }

        if (updateResult.IsFailed)
            return updateResult;

        await DbContext.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result> Handle(DeleteVehicleCommand command)
    {
        Result<Manager> getManager = await GetManagerAsync(command.RequestingManagerId, false);
        Result<Vehicle> getVehicle = await getManager.Bind(_ => GetVehicleAsync(command.VehicleId));
        Result<Enterprise> getEnterprise = getVehicle.Bind(v => GetAllowedToManagerEnterprise(getManager.Value, v.Enterprise.Id));
        if (getEnterprise.IsFailed)
            return getEnterprise.ToResult();

        Vehicle vehicle = getVehicle.Value;

        Result checkCanDelete = _vehiclesService.CheckCanDeleteVehicle(vehicle);
        if (checkCanDelete.IsFailed)
        {
            IEnumerable<IError> errors = checkCanDelete.Errors
                .Select(e => e is VehicleDomainError domainError ? VehiclesErrors.MapDomainError(domainError) : e);

            return Result.Fail(errors);
        }

        Result removeVehicle = await RemoveVehicleAsync(vehicle);

        return removeVehicle;
    }

    private async Task<Result<Vehicle>> GetVehicleAsync(Guid vehicleId)
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

    private static Result<Enterprise> GetAllowedToManagerEnterprise(Manager manager, Guid enterpriseId)
    {
        Enterprise? enterprise = manager.Enterprises.FirstOrDefault(e => e.Id == enterpriseId);

        return enterprise != null
            ? Result.Ok(enterprise)
            : Result.Fail<Enterprise>(VehiclesHandlersErrors.ManagerNotAllowedToEnterprise);
    }

    private async Task<Result<Model>> GetModelAsync(Guid modelId)
    {
        Model? model = await DbContext.Models.FirstOrDefaultAsync(m => m.Id == modelId);

        return model != null
            ? Result.Ok(model)
            : Result.Fail<Model>(VehiclesHandlersErrors.ModelNotExist);
    }

    private async Task<Result<List<Driver>>> GetAssignedDriversAsync(List<Guid> assignedDriverIds)
    {
        List<Driver> assignedDrivers = await DbContext.Drivers
            .Where(d => assignedDriverIds.Contains(d.Id))
            .ToListAsync();

        return assignedDrivers.Count == assignedDriverIds.Count
            ? Result.Ok(assignedDrivers)
            : Result.Fail<List<Driver>>(VehiclesHandlersErrors.AssignedDriversNotExist);
    }

    private async Task<Result<Driver?>> GetIfDefinedActiveAssignedDriverAsync(Guid? activeAssignedDriverId)
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

    private async Task<Result> RemoveVehicleAsync(Vehicle vehicle)
    {
        DbContext.Vehicles.Remove(vehicle);
        await DbContext.SaveChangesAsync();

        return Result.Ok();
    }
}