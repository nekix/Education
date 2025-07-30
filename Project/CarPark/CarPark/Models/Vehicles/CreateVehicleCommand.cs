using CarPark.Data;
using FluentResults;
using CarPark.Models.Drivers;
using Microsoft.EntityFrameworkCore;
using CarPark.Shared.CQ;

namespace CarPark.Models.Vehicles;

public class CreateVehicleCommand : ICommand<Result<int>>
{
    public required int ModelId { get; set; }

    public required int EnterpriseId { get; set; }

    public required string VinNumber { get; set; }

    public required decimal Price { get; set; }

    public required int ManufactureYear { get; set; }

    public required int Mileage { get; set; }

    public required string Color { get; set; }

    public required List<int> DriverIds { get; set; }

    public required int? ActiveDriverId { get; set; }

    public class Handler : ICommandHandler<CreateVehicleCommand, Result<int>>
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(CreateVehicleCommand command)
        {
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
                Color = command.Color
            };

            if (command.DriverIds.Count != 0)
            {
                List<Driver> drivers = await _context.Drivers
                    .Where(d => d.EnterpriseId == command.EnterpriseId)
                    .Where(d => command.DriverIds.Contains(d.Id))
                    .ToListAsync();

                if (drivers.Count != command.DriverIds.Count)
                {
                    return Result.Fail(Errors.InvalidDrivers);
                }

                vehicle.AssignedDrivers.AddRange(drivers);

                if (command.ActiveDriverId != null)
                {
                    int activeDriverId = command.ActiveDriverId.Value;

                    if (!command.DriverIds.Contains(activeDriverId))
                    {
                        return Result.Fail(Errors.InvalidActiveDriver);
                    }

                    vehicle.ActiveAssignedDriver = drivers.First(d => d.Id == activeDriverId);
                }
            }

            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();

            return vehicle.Id;
        }
    }

    public static class Errors
    {
        public const string InvalidDrivers = "InvalidDrivers";
        public const string InvalidActiveDriver = "InvalidActiveDriver";
    }
} 