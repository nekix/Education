using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.Rides.Queries;

public class GetRidesQuery : BaseManagerCommandQuery, IQuery<Result<RidesViewModel>>
{
    public required Guid VehicleId { get; set; }

    public required DateTimeOffset StartTime { get; set; }

    public required DateTimeOffset EndTime { get; set; }
}