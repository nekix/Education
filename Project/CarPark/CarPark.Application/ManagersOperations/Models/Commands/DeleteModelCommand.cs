using CarPark.Data;
using CarPark.Models;
using CarPark.Shared.CQ;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CarPark.ManagersOperations.Models.Commands;

public class DeleteModelCommand : ICommand<Result>
{
    public required Guid Id { get; set; }

    public class Handler : ICommandHandler<DeleteModelCommand, Result>
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteModelCommand command)
        {
            Model? model = await _context.Models.FindAsync(command.Id);

            if (model == null)
            {
                return Result.Fail(Errors.NotFound);
            }

            try
            {
                _context.Models.Remove(model);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23503" })
            {
                return new Error(Errors.Conflict)
                    .WithMetadata("ModelId", command.Id)
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