using CarPark.ManagersOperations.Drivers.Queries.Models;
using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.Drivers.Queries;

public class GetDriverQuery : IQuery<Result<DriverDto>>
{
    public required int RequestingManagerId { get; set; }

    public required int DriverId { get; set; }
}