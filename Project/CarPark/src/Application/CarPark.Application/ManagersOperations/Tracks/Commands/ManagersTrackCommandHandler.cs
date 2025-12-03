using CarPark.Data;
using CarPark.Managers;
using CarPark.ManagersOperations.Rides;
using CarPark.Rides;
using CarPark.Rides.Errors;
using CarPark.Rides.Services;
using CarPark.Vehicles;
using CarPark.Vehicles.Errors;
using CarPark.Vehicles.Services;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Xml;
using CarPark.CQ;
using CarPark.DateTimes;

namespace CarPark.ManagersOperations.Tracks.Commands;

internal class ManagersTrackCommandHandler : BaseManagersHandler,
    ICommandHandler<CreateRideFromGpxFileCommand, Result<Guid>>
{
    private readonly GeometryFactory _geometryFactory;
    private readonly IVehicleGeoTimePointsService _vehicleGeoTimePointsService;
    private readonly IRidesService _ridesService;

    public ManagersTrackCommandHandler(
        ApplicationDbContext dbContext,
        NtsGeometryServices services,
        IVehicleGeoTimePointsService vehicleGeoTimePointsService,
        IRidesService ridesService) : base(dbContext)
    {
        _geometryFactory = services.CreateGeometryFactory(new PrecisionModel(), 4326);
        _vehicleGeoTimePointsService = vehicleGeoTimePointsService;
        _ridesService = ridesService;
    }

    public async Task<Result<Guid>> Handle(CreateRideFromGpxFileCommand command)
    {
        Result<Manager> getManager = await GetManagerAsync(command.RequestingManagerId, false);
        Result<Vehicle> getVehicle = await getManager.Bind(_ => GetVehicleAsync(command.VehicleId));
        if (getVehicle.IsFailed)
        {
            return Result.Fail(getVehicle.Errors);
        }

        if (getManager.Value.Enterprises.All(e => e.Id != getVehicle.Value.Enterprise.Id))
        {
            return Result.Fail(TrackHandlersErrors.ManagerNotAllowedToVehicle);
        }

        using XmlReader reader = XmlReader.Create(command.GpxFileStream);

        GpxFile gpxFile = GpxFile.ReadFrom(reader, new GpxReaderSettings());

        if (gpxFile.Tracks.Count != 1)
            return Result.Fail(TrackHandlersErrors.GpxMustContainOneTrack);

        Result<List<VehicleGeoTimePoint>> conertToVehicleGeoTimePoint = ConvertToGeoTimePoints(gpxFile, getVehicle);
        if (conertToVehicleGeoTimePoint.IsFailed)
            return Result.Fail(conertToVehicleGeoTimePoint.Errors);

        List<VehicleGeoTimePoint> geoTimePoints = conertToVehicleGeoTimePoint.Value;

        if (geoTimePoints.Count < 2)
        {
            return Result.Fail(TrackHandlersErrors.GpxContainLessThanTwoPoints);
        }
        
        if (!CheckIsSequentialTrackTimeOrder(geoTimePoints))
        {
            return Result.Fail(TrackHandlersErrors.TracksNotSequential);
        }

        if (await CheckIsOverlapTrack(geoTimePoints, getVehicle.Value))
        {
            return Result.Fail(TrackHandlersErrors.TracksOverlapWithExisting);
        }

        if (await CheckIsOverlapRide(geoTimePoints, getVehicle.Value))
        {
            return Result.Fail(RidesHandlerErrors.RidesOverlapWithExisting);
        }

        await DbContext.VehicleGeoTimePoints.AddRangeAsync(geoTimePoints);

        VehicleGeoTimePoint startPoint = geoTimePoints.First();
        VehicleGeoTimePoint endPoint = geoTimePoints.Last();

        CreateRideRequest createRideRequest = new CreateRideRequest
        {
            Id = Guid.NewGuid(),
            Vehicle = getVehicle.Value,
            StartTime = new UtcDateTimeOffset(startPoint.Time.Value),
            EndTime = new UtcDateTimeOffset(endPoint.Time.Value),
            StartPoint = startPoint,
            EndPoint = endPoint
        };

        Result<Ride> createRideResult = _ridesService.CreateRide(createRideRequest);
        if (createRideResult.IsFailed)
        {
            IEnumerable<IError> errors = createRideResult.Errors
                .Select(e => e is RideDomainError domainError ? RidesErrors.MapDomainError(domainError) : e);

            return Result.Fail<Guid>(errors);
        }

        Ride ride = createRideResult.Value;

        await DbContext.Rides.AddAsync(ride);

        await DbContext.SaveChangesAsync();

        return Result.Ok(ride.Id);
    }



    private Result<List<VehicleGeoTimePoint>> ConvertToGeoTimePoints(GpxFile gpxFile, Result<Vehicle> getVehicle)
    {
        List<VehicleGeoTimePoint> geoTimePoints = new List<VehicleGeoTimePoint>();

        foreach (GpxWaypoint waypoint in gpxFile.Tracks.SelectMany(t => t.Segments).SelectMany(s => s.Waypoints))
        {
            Point point = _geometryFactory.CreatePoint(new Coordinate(waypoint.Longitude, waypoint.Latitude));

            DateTime? dtTime = waypoint.TimestampUtc;

            if (dtTime == null)
            {
                return Result.Fail<List<VehicleGeoTimePoint>>(TrackHandlersErrors.GpxPointsDateTimeNotDefined);
            }

            CreateVehicleGeoTimePointRequest createRequest = new CreateVehicleGeoTimePointRequest
            {
                Id = Guid.NewGuid(),
                Vehicle = getVehicle.Value,
                Location = point,
                Time = new UtcDateTimeOffset(dtTime.Value)
            };

            Result<VehicleGeoTimePoint> createGeoTimePoint = _vehicleGeoTimePointsService.CreateVehicleGeoTimePoint(createRequest);
            if (createGeoTimePoint.IsFailed)
            {
                IEnumerable<IError> errors = createGeoTimePoint.Errors
                    .Select(e => e is VehicleGeoTimePointDomainError domainError ? TrackErrors.VehicleGeoTimePoint.MapDomainError(domainError) : e);

                return Result.Fail<List<VehicleGeoTimePoint>>(errors);
            }

            geoTimePoints.Add(createGeoTimePoint.Value);
        }

        return Result.Ok<List<VehicleGeoTimePoint>>(geoTimePoints);
    }

    private static bool CheckIsSequentialTrackTimeOrder(IReadOnlyList<VehicleGeoTimePoint> geoTimePoints)
    {
        for (int i = 1; i < geoTimePoints.Count; i++)
            if (geoTimePoints[i].Time.Value <= geoTimePoints[i - 1].Time.Value)
                return false;

        return true;
    }

    private async Task<bool> CheckIsOverlapTrack(IReadOnlyCollection<VehicleGeoTimePoint> geoTimePoints, Vehicle vehicle)
    {
        VehicleGeoTimePoint startPoint = geoTimePoints.First();
        VehicleGeoTimePoint endPoint = geoTimePoints.Last();

        bool isOverlap = await DbContext.VehicleGeoTimePoints
            .AnyAsync(v => v.Vehicle.Id == vehicle.Id && v.Time >= startPoint.Time && v.Time <= endPoint.Time);

        return isOverlap;
    }

    private async Task<bool> CheckIsOverlapRide(IReadOnlyCollection<VehicleGeoTimePoint> geoTimePoints, Vehicle vehicle)
    {
        DateTimeOffset minNewTime = geoTimePoints.First().Time.Value;
        DateTimeOffset maxNewTime = geoTimePoints.Last().Time.Value;

        bool isOverlap = await DbContext.Rides
            .AnyAsync(r => r.Vehicle.Id == vehicle.Id &&
                          ((r.StartTime >= minNewTime && r.StartTime <= maxNewTime) ||
                           (r.EndTime >= minNewTime && r.EndTime <= maxNewTime) ||
                           (r.StartTime <= minNewTime && r.EndTime >= maxNewTime)));

        return isOverlap;
    }

    private async Task<Result<Vehicle>> GetVehicleAsync(Guid vehicleId)
    {
        Vehicle? vehicle = await DbContext.Vehicles
            .Include(v => v.Enterprise)
            .FirstOrDefaultAsync(v => v.Id == vehicleId);

        return vehicle != null
            ? Result.Ok(vehicle)
            : Result.Fail<Vehicle>(RidesHandlerErrors.VehicleNotFound);
    }
}