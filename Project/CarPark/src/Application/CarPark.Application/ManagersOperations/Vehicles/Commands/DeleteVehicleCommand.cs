using CarPark.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.Vehicles.Commands;

public class DeleteVehicleCommand : BaseManagerCommandQuery, ICommand<Result>
{
    public required Guid VehicleId { get; set; }
}