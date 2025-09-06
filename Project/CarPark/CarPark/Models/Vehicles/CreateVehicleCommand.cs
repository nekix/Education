using CarPark.Data;
using FluentResults;
using CarPark.Models.Drivers;
using Microsoft.EntityFrameworkCore;
using CarPark.Shared.CQ;
using CarPark.Models.Enterprises;
using CarPark.Models.Managers;

namespace CarPark.Models.Vehicles;

public class CreateVehicleCommand : ICommand<Result<int>>
{
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

    public required DateTimeOffset AddedToEnterpriseAt { get; set; }

    public class Handler : ICommandHandler<CreateVehicleCommand, Result<int>>
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(CreateVehicleCommand command)
        {
            if (command.AddedToEnterpriseAt < DateTimeOffset.Now)
                return Result.Fail<int>(Errors.AddedToEnterpriseDateGraterThenNow);

            // Управляет ли текущий менеджер предприятием, в котором меняет автомобиль
            Manager manager = await _context.Managers.FirstAsync(m => m.Id == command.RequestingManagerId);
            Enterprise enterprise = await _context.Enterprises
                .Include(e => e.Managers)
                .FirstAsync(e => e.Id == command.EnterpriseId);

            if (!enterprise.Managers.Contains(manager))
            {
                return Result.Fail<int>(Errors.ManagerNotInVehicleEnterprise);
            }

            Vehicle vehicle = new Vehicle
            {
                Id = default,
                ActiveAssignedDriver = null,
                AssignedDrivers = new List<Driver>(command.DriverIds.Count),
                ModelId = command.ModelId,
                EnterpriseId = command.EnterpriseId,
                VinNumber = command.VinNumber,
                Price = command.Price,
                ManufactureYear = command.ManufactureYear,
                Mileage = command.Mileage,
                Color = command.Color,
                AddedToEnterpriseAt = command.AddedToEnterpriseAt.ToUniversalTime()
            };

            // Если есть назначенные водители
            if (command.DriverIds.Count != 0)
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
                Result changeAssignedDrivers = SetAssignedDrivers(vehicle, newAssignedDrivers);
                if (changeAssignedDrivers.IsFailed)
                {
                    return Result.Fail<int>(changeAssignedDrivers.Errors);
                }
            }

            // Если есть назначенный водитель
            if (command.ActiveDriverId != null)
            {
                Driver? newActiveAssignedDriver = await _context.Drivers.FirstOrDefaultAsync(d => d.Id == command.ActiveDriverId);

                // Если не нашел нового активного водителя
                if (newActiveAssignedDriver == null)
                {
                    return Result.Fail<int>(Errors.NewActiveAssignedDriverNotFounded);
                }

                Result changeActiveAssignedDriver = SetActiveAssignedDriver(vehicle, newActiveAssignedDriver);
                if (changeActiveAssignedDriver.IsFailed)
                {
                    return Result.Fail<int>(changeActiveAssignedDriver.Errors);
                }
            }

            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();

            return vehicle.Id;
        }

        private Result SetAssignedDrivers(Vehicle vehicle, List<Driver> newAssignedDrivers)
        {
            // Нет водителей из других предприятий
            bool isDriversFromAnotherEnterprises = newAssignedDrivers.Any(d => d.EnterpriseId != vehicle.EnterpriseId);
            if (isDriversFromAnotherEnterprises)
            {
                return Result.Fail(Errors.AssignedDriversFromAnotherEnterprise);
            }

            vehicle.AssignedDrivers = newAssignedDrivers;

            return Result.Ok();
        }

        private Result SetActiveAssignedDriver(Vehicle vehicle, Driver? newActiveDriver)
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
    }

    public static class Errors
    {
        public const string AccessDenied = "AccessDenied";
        public const string ManagerNotInVehicleEnterprise = "ManagerNotInCurrentVehicleEnterprise";
        public const string AssignedDriversFromAnotherEnterprise = "AssignedDriversFromAnotherEnterprise";
        public const string NewAssignedDriversNotFounded = "NewAssignedDriversNotFounded";
        public const string NewActiveAssignedDriverNotFounded = "NewActiveAssignedDriverNotFounded";
        public const string NewActiveAssignedDriverNotInAssignedDrivers = "NewActiveAssignedDriverNotInAssignedDrivers";
        public const string DriverAlsoActiveAssignedToAnotherVehicle = "DriverAlsoActiveAssignedToAnotherVehicle";
        public const string AddedToEnterpriseDateGraterThenNow = "AddedToEnterpriseDateGraterThenNow";
    }
} 