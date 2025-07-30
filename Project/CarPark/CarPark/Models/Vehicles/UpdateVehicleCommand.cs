using CarPark.Data;
using FluentResults;
using CarPark.Models.Drivers;
using Microsoft.EntityFrameworkCore;
using CarPark.Shared.CQ;

namespace CarPark.Models.Vehicles;

public class UpdateVehicleCommand : ICommand<Result<int>>
{
    public required int Id { get; set; }

    public required int ModelId { get; set; }

    public required int EnterpriseId { get; set; }

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
            Vehicle? vehicle = await _context.Vehicles
                .Include(v => v.AssignedDrivers)
                .Include(v => v.ActiveAssignedDriver)
                .FirstOrDefaultAsync(v => v.Id == command.Id);

            if (vehicle == null)
            {
                return Result.Fail(Errors.NotFound);
            }

            // Entity Framework отслеживает изменения автоматически
            vehicle.ModelId = command.ModelId;
            vehicle.EnterpriseId = command.EnterpriseId;
            vehicle.VinNumber = command.VinNumber;
            vehicle.Price = command.Price;
            vehicle.ManufactureYear = command.ManufactureYear;
            vehicle.Mileage = command.Mileage;
            vehicle.Color = command.Color;

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

                vehicle.AssignedDrivers = drivers;

                if (command.ActiveDriverId != null)
                {
                    int activeDriverId = command.ActiveDriverId.Value;

                    if (!command.DriverIds.Contains(activeDriverId))
                    {
                        return Result.Fail(Errors.InvalidActiveDriver);
                    }

                    vehicle.ActiveAssignedDriver = drivers.First(d => d.Id == activeDriverId);
                }
                else
                {
                    vehicle.ActiveAssignedDriver = null;
                }
            }
            else
            {
                vehicle.AssignedDrivers.Clear();
                vehicle.ActiveAssignedDriver = null;
            }

            await _context.SaveChangesAsync();

            return Result.Ok(vehicle.Id);
        }
    }

    public static class Errors
    {
        public const string NotFound = "NotFound";
        public const string InvalidDrivers = "InvalidDrivers";
        public const string InvalidActiveDriver = "InvalidActiveDriver";
    }
} 