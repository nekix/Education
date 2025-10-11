﻿using CarPark.Shared.CQ;
using FluentResults;
using NetTopologySuite.Features;

namespace CarPark.ManagersOperations.Tracks.Queries;

public class GetRidesTrackFeatureCollectionQuery : BaseManagerCommandQuery, IQuery<Result<FeatureCollection>>
{
    public required Guid VehicleId { get; set; }

    public required DateTimeOffset StartTime { get; set; }

    public required DateTimeOffset EndTime { get; set; }
}