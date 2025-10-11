﻿using CarPark.ManagersOperations.Drivers.Queries.Models;
using CarPark.Shared.CQ;
using FluentResults;

namespace CarPark.ManagersOperations.Drivers.Queries;

public class GetDriverQuery : IQuery<Result<DriverDto>>
{
    public required Guid RequestingManagerId { get; set; }

    public required Guid DriverId { get; set; }
}