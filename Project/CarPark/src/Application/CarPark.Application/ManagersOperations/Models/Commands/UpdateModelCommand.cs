using CarPark.CQ;
using CarPark.Data;
using CarPark.Models;
using CarPark.Models.Errors;
using CarPark.Models.Services;
using FluentResults;
using CarPark.Errors;

namespace CarPark.ManagersOperations.Models.Commands;

public class UpdateModelCommand : ICommand<Result<Guid>>
{
    public required Guid Id { get; set; }

    public required string ModelName { get; set; }

    public required string VehicleType { get; set; }

    public required int SeatsCount { get; set; }

    public required double MaxLoadingWeightKg { get; set; }

    public required double EnginePowerKW { get; set; }

    public required string TransmissionType { get; set; }

    public required string FuelSystemType { get; set; }

    public required string FuelTankVolumeLiters { get; set; }

    public class Handler : ICommandHandler<UpdateModelCommand, Result<Guid>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IModelsService _modelsService;

        public Handler(ApplicationDbContext context, IModelsService modelsService)
        {
            _context = context;
            _modelsService = modelsService;
        }

        public async Task<Result<Guid>> Handle(UpdateModelCommand command)
        {
            Model? model = await _context.Models.FindAsync(command.Id);

            if (model == null)
            {
                return Result.Fail<Guid>(new WebApiError(404, "Model not found."));
            }

            UpdateModelRequest request = new UpdateModelRequest
            {
                ModelName = command.ModelName,
                VehicleType = command.VehicleType,
                SeatsCount = command.SeatsCount,
                MaxLoadingWeightKg = command.MaxLoadingWeightKg,
                EnginePowerKW = command.EnginePowerKW,
                TransmissionType = command.TransmissionType,
                FuelSystemType = command.FuelSystemType,
                FuelTankVolumeLiters = command.FuelTankVolumeLiters
            };

            Result updateModel = _modelsService.UpdateModel(model, request);
            if (updateModel.IsFailed)
            {
                IEnumerable<IError> errors = updateModel.Errors
                    .Select(e => e is ModelDomainError domainError ? ModelsErrors.MapDomainError(domainError) : e);

                return Result.Fail(errors);
            }

            await _context.SaveChangesAsync();

            return model.Id;
        }
    }

}