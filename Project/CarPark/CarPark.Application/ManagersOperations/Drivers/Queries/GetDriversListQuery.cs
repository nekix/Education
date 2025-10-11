using CarPark.ManagersOperations.Drivers.Queries.Models;
using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.Drivers.Queries;

public class GetDriversListQuery : IQuery<Result<PaginatedDrivers>>
{
    public required Guid RequestingManagerId { get; set; }

    public required uint Limit { get; init; }

    public required uint Offset { get; init; }
}