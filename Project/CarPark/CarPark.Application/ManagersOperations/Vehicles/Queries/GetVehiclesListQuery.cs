using CarPark.ManagersOperations.Vehicles.Queries.Models;
using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.Vehicles.Queries;

public class GetVehiclesListQuery : BaseManagerCommandQuery, IQuery<Result<PaginatedVehicles>>
{
    public required uint Limit { get; init; }

    public required uint Offset { get; init; }
}