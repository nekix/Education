using CarPark.CQ;
using CarPark.Data;
using CarPark.Errors;
using CarPark.Models;
using CarPark.Models.Errors;
using CarPark.Models.Services;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using CarPark.Vehicles;

namespace CarPark.ManagersOperations.Models.Commands;

public class DeleteModelCommand : ICommand<Result>
{
    public required Guid Id { get; set; }

    public class Handler : ICommandHandler<DeleteModelCommand, Result>
    {
        private readonly ApplicationDbContext _context;
        private readonly IModelsService _modelsService;

        public Handler(ApplicationDbContext context, IModelsService modelsService)
        {
            _context = context;
            _modelsService = modelsService;
        }

        public async Task<Result> Handle(DeleteModelCommand command)
        {
            Model? model = await _context.Models.FindAsync(command.Id);

            if (model == null)
            {
                return Result.Fail(new WebApiError(404, "Model not found."));
            }

            List<Vehicle> vehicles = await _context.Vehicles
                .Where(v => v.Model.Id == model.Id).ToListAsync();

            Result checkCanDeleteModel = _modelsService.CheckCanDeleteModel(model, vehicles);
            if (checkCanDeleteModel.IsFailed)
            {
                IEnumerable<IError> errors = checkCanDeleteModel.Errors
                    .Select(e => e is ModelDomainError domainError ? ModelsErrors.MapDomainError(domainError) : e);
                return Result.Fail(errors);
            }

            try
            {
                _context.Models.Remove(model);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23503" })
            {
                return Result.Fail(new WebApiError(409, "Cannot delete model because it is referenced by vehicles.")
                    .WithMetadata("ModelId", command.Id)
                    .CausedBy(new ExceptionalError(ex)));
            }

            return Result.Ok();
        }
    }

}