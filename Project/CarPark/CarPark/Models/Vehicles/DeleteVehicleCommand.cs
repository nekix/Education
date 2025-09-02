using CarPark.Data;
using CarPark.Shared.CQ;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CarPark.Models.Vehicles;

public class DeleteVehicleCommand : ICommand<Result>
{
    public required int Id { get; set; }
    public required int RequestingManagerId { get; set; }

    public class Handler : ICommandHandler<DeleteVehicleCommand, Result>
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteVehicleCommand command)
        {
            Vehicle? vehicle = await _context.Vehicles
                .Include(v => v.AssignedDrivers)
                .Include(v => v.ActiveAssignedDriver)
                .FirstOrDefaultAsync(v => v.Id == command.Id);

            if (vehicle == null)
            {
                return Result.Fail(Errors.NotFound);
            }

            // Проверка доступа к автомобилю
            bool hasAccess = await _context.Enterprises
                .AnyAsync(e => e.Id == vehicle.EnterpriseId && 
                              e.Managers.Any(m => m.Id == command.RequestingManagerId));
            if (!hasAccess)
            {
                return Result.Fail(Errors.AccessDenied);
            }

            if (vehicle.AssignedDrivers.Count != 0)
            {
                return Result.Fail(Errors.HasAssignedDrivers);
            }

            try
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23503" })
            {
                return new Error(Errors.Conflict)
                    .WithMetadata("VehicleId", command.Id)
                    .CausedBy(new ExceptionalError(ex));
            }

            return Result.Ok();
        }
    }

    public static class Errors
    {
        public const string NotFound = "NotFound";
        public const string Conflict = "Conflict";
        public const string AccessDenied = "AccessDenied";
        public const string HasAssignedDrivers = "HasAssignedDrivers";
    }
} 