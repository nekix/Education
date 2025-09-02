using CarPark.Data;
using FluentResults;
using CarPark.Models.Drivers;
using CarPark.Models.Enterprises;
using CarPark.Models.Managers;
using CarPark.Models.Models;
using Microsoft.EntityFrameworkCore;
using CarPark.Shared.CQ;

namespace CarPark.Models.Vehicles;

public class UpdateVehicleCommand : ICommand<Result<int>>
{
    public required int Id { get; set; }

    public required int ModelId { get; set; }

    public required int EnterpriseId { get; set; }

    public required int RequestingManagerId { get; set; }

    public required string VinNumber { get; set; }

    public required decimal Price { get; set; }

    public required int ManufactureYear { get; set; }

    public required int Mileage { get; set; }

    public required string Color { get; set; }

    public required List<int> DriverIds { get; set; }

    public required int? ActiveDriverId { get; set; }

    public class Handler : ICommandHandler<UpdateVehicleCommand, Result<int>>
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(UpdateVehicleCommand command)
        {
            // Сущетсвует ли эта машина
            Vehicle? vehicle = await _context.Vehicles
                .Include(v => v.AssignedDrivers)
                .Include(v => v.ActiveAssignedDriver)
                .FirstOrDefaultAsync(v => v.Id == command.Id);

            if (vehicle == null)
            {
                return Result.Fail<int>(Errors.VehicleNotFound);
            }

            // Управляет ли текущий менеджер предприятием, в котором меняет автомобиль
            Manager manager = await _context.Managers.FirstAsync(m => m.Id == command.RequestingManagerId);
            Enterprise enterprise = await _context.Enterprises
                .Include(e => e.Managers)
                .FirstAsync(e => e.Id == vehicle.EnterpriseId);

            if (!enterprise.Managers.Contains(manager))
            {
                return Result.Fail<int>(Errors.ManagerNotInCurrentVehicleEnterprise);
            }

            // Если меняется предприятие
            if (vehicle.EnterpriseId != command.EnterpriseId)
            {
                // Существует ли новое предприятие
                Enterprise? newEnterprise = await _context.Enterprises
                    .Include(e => e.Managers)
                    .FirstOrDefaultAsync(e => e.Id == command.EnterpriseId);

                if (newEnterprise == null)
                {
                    return Result.Fail<int>(Errors.NewEnterpriseNotFound);
                }

                // Бизнес проверки на смену предприятия
                Result changeEnterprise = ChangeEnterprise(vehicle, newEnterprise, manager);
                if (changeEnterprise.IsFailed)
                {
                    return Result.Fail<int>(changeEnterprise.Errors);
                }
            }

            // Если состав назначенных водителей изменился
            if (vehicle.AssignedDrivers.Count != command.DriverIds.Count || 
                vehicle.AssignedDrivers.Select(d => d.Id).Intersect(command.DriverIds).Count() != vehicle.AssignedDrivers.Count)
            {
                List<Driver> newAssignedDrivers = await _context.Drivers
                    .Where(d => command.DriverIds.Any(id => id == d.Id))
                    .ToListAsync();

                // Если не нашел часть водителей
                if (newAssignedDrivers.Count != command.DriverIds.Count)
                {
                    return Result.Fail<int>(Errors.NewAssignedDriversNotFounded);
                }

                // Бизнес проверки на смена назначенных водителей
                Result changeAssignedDrivers = ChangeAssignedDrivers(vehicle, newAssignedDrivers);
                if (changeAssignedDrivers.IsFailed)
                {
                    return Result.Fail<int>(changeAssignedDrivers.Errors);
                }
            }

            // Если изменился назначенный водитель
            if (vehicle.ActiveAssignedDriver?.Id != command.ActiveDriverId)
            {
                Driver? newActiveAssignedDriver = await _context.Drivers.FirstOrDefaultAsync(d => d.Id == command.ActiveDriverId);

                // Если не нашел нового активного водителя
                if (newActiveAssignedDriver == null && command.ActiveDriverId != null)
                {
                    return Result.Fail<int>(Errors.NewActiveAssignedDriverNotFounded);
                }

                Result changeActiveAssignedDriver = ChangeActiveAssignedDriver(vehicle, newActiveAssignedDriver);
                if (changeActiveAssignedDriver.IsFailed)
                {
                    return Result.Fail<int>(changeActiveAssignedDriver.Errors);
                }
            }

            if (vehicle.ModelId != command.ModelId)
            {
                Model? model = await _context.Models.FirstOrDefaultAsync(m => m.Id == command.ModelId);
                if (model == null)
                {
                    return Result.Fail(Errors.NewModelNotFounded);
                }

                vehicle.ModelId = command.ModelId;
            }

            vehicle.VinNumber = command.VinNumber;
            vehicle.Price = command.Price;
            vehicle.ManufactureYear = command.ManufactureYear;
            vehicle.Mileage = command.Mileage;
            vehicle.Color = command.Color;

            await _context.SaveChangesAsync();

            return Result.Ok(vehicle.Id);
        }

        private Result ChangeAssignedDrivers(Vehicle vehicle, List<Driver> newAssignedDrivers)
        {
            // Нет водителей из других предприятий
            bool isDriversFromAnotherEnterprises = newAssignedDrivers.Any(d => d.EnterpriseId != vehicle.EnterpriseId);
            if (isDriversFromAnotherEnterprises)
            {
                return Result.Fail(Errors.AssignedDriversFromAnotherEnterprise);
            }

            // Среди исключенных водителей нет активного в данный момент для данной машины
            bool isExcludeActiveDriver = vehicle.AssignedDrivers
                .Except(newAssignedDrivers)
                .Contains(vehicle.ActiveAssignedDriver);
            if (isExcludeActiveDriver)
            {
                return Result.Fail(Errors.ExcludeActiveAssignedDriverFromAssigned);
            }

            vehicle.AssignedDrivers = newAssignedDrivers;

            return Result.Ok();
        }

        private Result ChangeActiveAssignedDriver(Vehicle vehicle, Driver? newActiveDriver)
        {
            if (newActiveDriver != null)
            {
                if (newActiveDriver.ActiveAssignedVehicle != null)
                {
                    return Result.Fail(Errors.DriverAlsoActiveAssignedToAnotherVehicle);
                }

                if (!vehicle.AssignedDrivers.Contains(newActiveDriver))
                {
                    return Result.Fail(Errors.NewActiveAssignedDriverNotInAssignedDrivers);
                }
            }

            vehicle.ActiveAssignedDriver = newActiveDriver;

            return Result.Ok();
        }

        private Result ChangeEnterprise(Vehicle vehicle, Enterprise newEnterprise, Manager requestedManager)
        {
            // Менеджер имеет доступ к новому предприятию
            if (!newEnterprise.Managers.Contains(requestedManager))
            {
                return Result.Fail(Errors.ManagerNotInNewEnterprise);
            }

            // Нет назначенных водителей
            if (vehicle.AssignedDrivers.Any())
            {
                return Result.Fail(Errors.HasAssignedDrivers);
            }

            vehicle.EnterpriseId = newEnterprise.Id;

            return Result.Ok();
        }
    }

    public static class Errors
    {
        public const string VehicleNotFound = "NotFound";
        public const string ExcludeActiveAssignedDriverFromAssigned = "ExcludeActiveAssignedDriverFromAssigned";
        public const string NewActiveAssignedDriverNotInAssignedDrivers = "NewActiveAssignedDriverNotInAssignedDrivers";
        public const string DriverAlsoActiveAssignedToAnotherVehicle = "DriverAlsoActiveAssignedToAnotherVehicle";
        public const string AssignedDriversFromAnotherEnterprise = "AssignedDriversFromAnotherEnterprise";
        public const string NewActiveAssignedDriverNotFounded = "NewActiveAssignedDriverNotFounded";
        public const string ManagerNotInCurrentVehicleEnterprise = "ManagerNotInCurrentVehicleEnterprise";
        public const string NewEnterpriseNotFound = "NewEnterpriseNotFound";
        public const string NewAssignedDriversNotFounded = "NewAssignedDriversNotFounded";
        public const string ManagerNotInNewEnterprise = "ManagerNotInNewEnterprise";
        public const string HasAssignedDrivers = "HasAssignedDrivers";
        public const string NewModelNotFounded = "NewModelNotFounded";
    }
} 