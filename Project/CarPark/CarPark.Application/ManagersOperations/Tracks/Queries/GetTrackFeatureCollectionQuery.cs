﻿using CarPark.Shared.CQ;
using FluentResults;
using NetTopologySuite.Features;

namespace CarPark.ManagersOperations.Tracks.Queries;

public class GetTrackFeatureCollectionQuery : BaseManagerCommandQuery, IQuery<Result<FeatureCollection>>
{
    public required int VehicleId { get; set; }

    public required DateTimeOffset StartTime { get; set; }

    public required DateTimeOffset EndTime { get; set; }
}