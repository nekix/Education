using CarPark.ManagersOperations.Vehicles.Queries.Models;
using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.Vehicles.Queries;

public class GetVehicleQuery : BaseManagerCommandQuery, IQuery<Result<VehicleDto>>
{
    public required int VehicleId { get; set; }
}