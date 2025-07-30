using CarPark.Data;
using CarPark.Models.Models;
using CarPark.Shared.CQ;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CarPark.Models.Vehicles;

public class DeleteVehicleCommand : ICommand<Result>
{
    public required int Id { get; set; }

    public class Handler : ICommandHandler<DeleteVehicleCommand, Result>
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteVehicleCommand command)
        {
            Vehicle? vehicle = await _context.Vehicles.FindAsync(command.Id);

            if (vehicle == null)
            {
                return Result.Fail(Errors.NotFound);
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
    }
} 