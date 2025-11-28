using CarPark.CQ;
using CarPark.ManagersOperations.Vehicles.Queries.Models;
using FluentResults;

namespace CarPark.ManagersOperations.Vehicles.Queries;

public class GetVehicleQuery : BaseManagerCommandQuery, IQuery<Result<VehicleDto>>
{
    public required Guid VehicleId { get; set; }
}