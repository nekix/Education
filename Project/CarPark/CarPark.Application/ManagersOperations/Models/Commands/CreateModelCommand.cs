using CarPark.CQ;
using CarPark.Data;
using CarPark.Models;
using CarPark.Models.Errors;
using CarPark.Models.Services;
using FluentResults;
using CarPark.Errors;

namespace CarPark.ManagersOperations.Models.Commands;

public class CreateModelCommand : ICommand<Result<Guid>>
{
    public required string ModelName { get; set; }

    public required string VehicleType { get; set; }

    public required int SeatsCount { get; set; }

    public required double MaxLoadingWeightKg { get; set; }

    public required double EnginePowerKW { get; set; }

    public required string TransmissionType { get; set; }

    public required string FuelSystemType { get; set; }

    public required string FuelTankVolumeLiters { get; set; }

    public class Handler : ICommandHandler<CreateModelCommand, Result<Guid>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IModelsService _modelsService;

        public Handler(ApplicationDbContext context,
            IModelsService modelService)
        {
            _context = context;
            _modelsService = modelService;
        }

        public async Task<Result<Guid>> Handle(CreateModelCommand command)
        {
            CreateModelRequest request = new CreateModelRequest
            {
                Id = default,
                ModelName = command.ModelName,
                VehicleType = command.VehicleType,
                SeatsCount = command.SeatsCount,
                MaxLoadingWeightKg = command.MaxLoadingWeightKg,
                EnginePowerKW = command.EnginePowerKW,
                TransmissionType = command.TransmissionType,
                FuelSystemType = command.FuelSystemType,
                FuelTankVolumeLiters = command.FuelTankVolumeLiters
            };

            Result<Model> createModel = _modelsService.CreateModel(request);
            if (createModel.IsFailed)
            {
                IEnumerable<WebApiError> apiErrors = createModel.Errors
                    .OfType<ModelDomainError>()
                    .Select(ModelsErrors.MapDomainError);

                return Result.Fail<Guid>(apiErrors);
            }

            Model model = createModel.Value;

            await _context.Models.AddAsync(model);
            await _context.SaveChangesAsync();

            return Result.Ok(model.Id);
        }
    }
}