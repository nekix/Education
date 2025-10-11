using CarPark.Data;
using CarPark.Models;
using CarPark.Shared.CQ;
using FluentResults;

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

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(CreateModelCommand command)
        {
            Model model = new Model
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

            await _context.Models.AddAsync(model);
            await _context.SaveChangesAsync();

            return model.Id;
        }
    }
}