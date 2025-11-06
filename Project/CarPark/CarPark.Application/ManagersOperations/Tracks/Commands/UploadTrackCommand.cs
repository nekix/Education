using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.Tracks.Commands;

public class CreateRideFromGpxFileCommand : BaseManagerCommandQuery,
    ICommand<Result<Guid>>
{
    public required Guid VehicleId { get; init; }

    public required Stream GpxFileStream { get; init; }
}