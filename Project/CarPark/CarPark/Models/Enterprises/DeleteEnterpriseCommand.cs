using CarPark.Data;
using CarPark.Shared.CQ;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CarPark.Models.Enterprises;

public class DeleteEnterpriseCommand : ICommand<Result>
{
    public required int Id { get; set; }
    public required int RequestingManagerId { get; set; }

    public class Handler : ICommandHandler<DeleteEnterpriseCommand, Result>
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteEnterpriseCommand command)
        {
            Enterprise? enterprise = await _context.Enterprises
                .Include(e => e.Managers)
                .FirstOrDefaultAsync(e => e.Id == command.Id);

            if (enterprise == null)
            {
                return Result.Fail(Errors.NotFound);
            }

            // Проверяем, что запрашивающий менеджер имеет доступ к этому предприятию
            if (!enterprise.Managers.Any(m => m.Id == command.RequestingManagerId))
            {
                return Result.Fail(Errors.AccessDenied);
            }

            // Проверяем, что предприятие видимо другим менеджерам
            if (enterprise.Managers.Count > 1)
            {
                return Result.Fail(Errors.VisibleToOtherManagers);
            }

            // Проверяем, что в предприятии нет автомобилей
            bool hasVehicles = await _context.Vehicles
                .AnyAsync(v => v.EnterpriseId == command.Id);

            if (hasVehicles)
            {
                return Result.Fail(Errors.HasVehicles);
            }

            // Проверяем, что в предприятии нет водителей
            bool hasDrivers = await _context.Drivers
                .AnyAsync(d => d.EnterpriseId == command.Id);

            if (hasDrivers)
            {
                return Result.Fail(Errors.HasDrivers);
            }

            try
            {
                _context.Enterprises.Remove(enterprise);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23503" })
            {
                return new Error(Errors.Conflict)
                    .WithMetadata("EnterpriseId", command.Id)
                    .CausedBy(new ExceptionalError(ex));
            }

            return Result.Ok();
        }
    }

    public static class Errors
    {
        public const string NotFound = "NotFound";
        public const string AccessDenied = "AccessDenied";
        public const string VisibleToOtherManagers = "VisibleToOtherManagers";
        public const string HasVehicles = "HasVehicles";
        public const string HasDrivers = "HasDrivers";
        public const string Conflict = "Conflict";
    }
}
