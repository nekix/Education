﻿namespace CarPark.Services.GeoServices.GeoCodingServices;

public class GeoDetails
{
    public required string Address { get; init; }

    public required string Latitude { get; init; }

    public required string Longitude { get; init; }
}