using CarPark.Data;
using CarPark.Models;
using CarPark.Shared.CQ;
using FluentResults;

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

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(UpdateModelCommand command)
        {
            Model? model = await _context.Models.FindAsync(command.Id);

            if (model == null)
            {
                return Result.Fail(Errors.NotFound);
            }

            if (model.ModelName != command.ModelName)
                model.ModelName = command.ModelName;

            if (model.VehicleType != command.VehicleType)
                model.VehicleType = command.VehicleType;

            if (model.SeatsCount != command.SeatsCount)
                model.SeatsCount = command.SeatsCount;

            if (model.MaxLoadingWeightKg != command.MaxLoadingWeightKg)
                model.MaxLoadingWeightKg = command.MaxLoadingWeightKg;

            if (model.EnginePowerKW != command.EnginePowerKW)
                model.EnginePowerKW = command.EnginePowerKW;

            if (model.TransmissionType != command.TransmissionType)
                model.TransmissionType = command.TransmissionType;

            if (model.FuelSystemType != command.FuelSystemType)
                model.FuelSystemType = command.FuelSystemType;

            if (model.FuelTankVolumeLiters != command.FuelTankVolumeLiters)
                model.FuelTankVolumeLiters = command.FuelTankVolumeLiters;

            await _context.SaveChangesAsync();

            return model.Id;
        }
    }

    public static class Errors
    {
        public const string NotFound = "NotFound";
    }
}