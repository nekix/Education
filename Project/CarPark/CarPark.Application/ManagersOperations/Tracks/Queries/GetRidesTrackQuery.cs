using CarPark.ManagersOperations.Tracks.Queries.Models;
using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.Tracks.Queries;

public class GetRidesTrackQuery : BaseManagerCommandQuery, IQuery<Result<TrackViewModel>>
{
    public required int VehicleId { get; set; }

    public required DateTimeOffset StartTime { get; set; }

    public required DateTimeOffset EndTime { get; set; }
}