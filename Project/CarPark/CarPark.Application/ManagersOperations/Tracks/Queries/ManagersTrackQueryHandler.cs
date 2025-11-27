using CarPark.CQ;
using CarPark.Data;
using CarPark.Managers;
using CarPark.ManagersOperations.Tracks.Queries.Models;
using CarPark.Rides;
using CarPark.TimeZones;
using CarPark.TimeZones.Conversion;
using CarPark.Vehicles;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace CarPark.ManagersOperations.Tracks.Queries;

public class ManagersTrackQueryHandler : BaseManagersHandler,
    IQueryHandler<GetTrackQuery, Result<TrackViewModel>>,
    IQueryHandler<GetTrackFeatureCollectionQuery, Result<FeatureCollection>>,
    IQueryHandler<GetRidesTrackQuery, Result<TrackViewModel>>,
    IQueryHandler<GetRidesTrackFeatureCollectionQuery, Result<FeatureCollection>>
{
    private readonly ITimeZoneConversionService _timeZoneService;

    public ManagersTrackQueryHandler(ApplicationDbContext dbContext,
        ITimeZoneConversionService timeZoneService) 
        : base(dbContext)
    {
        _timeZoneService = timeZoneService;
    }

    public async Task<Result<TrackViewModel>> Handle(GetTrackQuery query)
    {
        Result<List<IntermediateGeoTimePoint>> getIntGeoTimePoints =
            await GetAllIntGeoTimePoints(query.RequestingManagerId, query.VehicleId, query.StartTime, query.EndTime);

        if (getIntGeoTimePoints.IsFailed)
            return getIntGeoTimePoints.ToResult<TrackViewModel>();

        List<GeoTimePoint> geoTimePoints = getIntGeoTimePoints.Value
            .Select(p => new GeoTimePoint
            {
                Time = p.Time,
                X = p.Point.X,
                Y = p.Point.Y
            }).ToList();

        TrackViewModel viewModel = new TrackViewModel
        {
            GeoTimePoints = geoTimePoints
        };

        return Result.Ok(viewModel);
    }

    public async Task<Result<FeatureCollection>> Handle(GetTrackFeatureCollectionQuery query)
    {
        Result<List<IntermediateGeoTimePoint>> getIntGeoTimePoints = 
            await GetAllIntGeoTimePoints(query.RequestingManagerId, query.VehicleId, query.StartTime, query.EndTime);

        if (getIntGeoTimePoints.IsFailed)
            return getIntGeoTimePoints.ToResult<FeatureCollection>();

        List<Feature> features = getIntGeoTimePoints.Value
            .Select(p => new Feature
            {
                Geometry = p.Point,
                Attributes = new AttributesTable
                {
                    { "time", p.Time.ToString("O") },
                    { "timestamp", p.Time.ToUnixTimeSeconds() }
                }
            }).ToList();

        FeatureCollection featureCollection = new FeatureCollection(features);

        return Result.Ok(featureCollection);
    }

    public async Task<Result<TrackViewModel>> Handle(GetRidesTrackQuery query)
    {
        Result<List<IntermediateGeoTimePoint>> getIntGeoTimePoints =
            await GetInRidesIntGeoTimePoints(query.RequestingManagerId, query.VehicleId, query.StartTime, query.EndTime);

        if (getIntGeoTimePoints.IsFailed)
            return getIntGeoTimePoints.ToResult<TrackViewModel>();

        List<GeoTimePoint> geoTimePoints = getIntGeoTimePoints.Value
            .Select(p => new GeoTimePoint
            {
                Time = p.Time,
                X = p.Point.X,
                Y = p.Point.Y
            }).ToList();

        TrackViewModel viewModel = new TrackViewModel
        {
            GeoTimePoints = geoTimePoints
        };

        return Result.Ok(viewModel);
    }

    public async Task<Result<FeatureCollection>> Handle(GetRidesTrackFeatureCollectionQuery query)
    {
        Result<List<IntermediateGeoTimePoint>> getIntGeoTimePoints =
            await GetInRidesIntGeoTimePoints(query.RequestingManagerId, query.VehicleId, query.StartTime, query.EndTime);

        if (getIntGeoTimePoints.IsFailed)
            return getIntGeoTimePoints.ToResult<FeatureCollection>();

        List<Feature> features = getIntGeoTimePoints.Value
            .Select(p => new Feature
            {
                Geometry = p.Point,
                Attributes = new AttributesTable
                {
                    { "time", p.Time.ToString("O") },
                    { "timestamp", p.Time.ToUnixTimeSeconds() }
                }
            }).ToList();

        FeatureCollection featureCollection = new FeatureCollection(features);

        return Result.Ok(featureCollection);
    }

    private async Task<Result<List<IntermediateGeoTimePoint>>> GetInRidesIntGeoTimePoints(
        Guid managerId,
        Guid vehicleId,
        DateTimeOffset startTime,
        DateTimeOffset endTime)
    {
        Result<Manager> getManager = await GetManagerAsync(managerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult<List<IntermediateGeoTimePoint>>();

        Manager manager = getManager.Value;

        Vehicle? vehicle = await DbContext.Vehicles
            .Include(v => v.Enterprise)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == vehicleId);

        if (vehicle == null)
            return Result.Fail<List<IntermediateGeoTimePoint>>(TrackHandlersErrors.VehicleNotFound);

        if (manager.Enterprises.All(e => e.Id != vehicle.Enterprise.Id))
            return Result.Fail<List<IntermediateGeoTimePoint>>(TrackHandlersErrors.ManagerNotAllowedToVehicle);

        IQueryable<Ride> ridesQuery = DbContext.Rides
            .Where(r => r.Vehicle.Id == vehicle.Id
                        && r.StartTime >= startTime.ToUniversalTime()
                        && r.EndTime <= endTime.ToUniversalTime());

        List<IntermediateGeoTimePoint> intGeoTimePoints = await DbContext.VehicleGeoTimePoints
            .Where(p => p.Vehicle.Id == vehicle.Id)
            .Where(p => ridesQuery.Any(r => r.StartTime <= p.Time && r.EndTime >= p.Time))
            .Where(p => p.Time >= startTime.ToUniversalTime()
                        && p.Time <= endTime.ToUniversalTime())
            .OrderBy(p => p.Time)
            .Select(p => new IntermediateGeoTimePoint
            {
                Time = p.Time,
                Point = p.Location
            })
            .ToListAsync();

        TzInfo? enterpriseTimeZone = await DbContext.Enterprises
            .Where(e => e.Id == vehicle.Enterprise.Id)
            .Select(e => e.TimeZone)
            .FirstOrDefaultAsync();

        foreach (IntermediateGeoTimePoint point in intGeoTimePoints)
        {
            point.Time = _timeZoneService.ConvertToEnterpriseTimeZone(point.Time, enterpriseTimeZone);
        }

        return Result.Ok(intGeoTimePoints);
    }

    private async Task<Result<List<IntermediateGeoTimePoint>>> GetAllIntGeoTimePoints(
        Guid managerId,
        Guid vehicleId, 
        DateTimeOffset startTime, 
        DateTimeOffset endTime)
    {
        Result<Manager> getManager = await GetManagerAsync(managerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult<List<IntermediateGeoTimePoint>>();

        Manager manager = getManager.Value;

        Vehicle? vehicle = await DbContext.Vehicles
            .Include(v => v.Enterprise)
            .FirstOrDefaultAsync(v => v.Id == vehicleId);

        if (vehicle == null)
            return Result.Fail<List<IntermediateGeoTimePoint>>(TrackHandlersErrors.VehicleNotFound);

        if (manager.Enterprises.All(e => e.Id != vehicle.Enterprise.Id))
            return Result.Fail<List<IntermediateGeoTimePoint>>(TrackHandlersErrors.ManagerNotAllowedToVehicle);

        List<IntermediateGeoTimePoint> intGeoTimePoints = await DbContext.VehicleGeoTimePoints
            .Where(p => p.Vehicle.Id == vehicle.Id)
            .Where(p => p.Time >= startTime.ToUniversalTime()
                        && p.Time <= endTime.ToUniversalTime())
            .OrderBy(p => p.Time)
            .Select(p => new IntermediateGeoTimePoint
            {
                Time = p.Time,
                Point = p.Location
            })
            .ToListAsync();

        TzInfo? enterpriseTimeZone = await DbContext.Enterprises
            .Where(e => e.Id == vehicle.Enterprise.Id)
            .Select(e => e.TimeZone)
            .FirstOrDefaultAsync();

        foreach (IntermediateGeoTimePoint point in intGeoTimePoints)
        {
            point.Time = _timeZoneService.ConvertToEnterpriseTimeZone(point.Time, enterpriseTimeZone);
        }

        return Result.Ok(intGeoTimePoints);
    }

    private class IntermediateGeoTimePoint
    {
        public required DateTimeOffset Time { get; set; }

        public required Point Point { get; set; }
    }
}