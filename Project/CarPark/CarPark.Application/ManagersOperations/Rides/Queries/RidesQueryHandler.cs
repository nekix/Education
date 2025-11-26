using CarPark.Application.Geo.GeoCoding;
using CarPark.Data;
using CarPark.Managers;
using CarPark.Rides;
using CarPark.Shared.CQ;
using CarPark.TimeZones;
using CarPark.TimeZones.Conversion;
using CarPark.Vehicles;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CarPark.ManagersOperations.Rides.Queries;

public class RidesQueryHandler : BaseManagersHandler,
    IQueryHandler<GetRidesQuery, Result<RidesViewModel>>
{
    private readonly ITimeZoneConversionService _timeZoneService;
    private readonly IGeoCodingService _geoCodingService;

    public RidesQueryHandler(ApplicationDbContext dbContext,
        ITimeZoneConversionService timeZoneService,
        IGeoCodingService geoCodingService) 
        : base(dbContext)
    {
        _timeZoneService = timeZoneService;
        _geoCodingService = geoCodingService;
    }

    public async Task<Result<RidesViewModel>> Handle(GetRidesQuery query)
    {
        Result<Manager> getManager = await GetManagerAsync(query.RequestingManagerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult<RidesViewModel>();

        Manager manager = getManager.Value;

        Vehicle? vehicle = await DbContext.Vehicles
            .Include(v => v.Enterprise)
            .FirstOrDefaultAsync(v => v.Id == query.VehicleId);

        if (vehicle == null)
            return Result.Fail<RidesViewModel>(RidesHandlerErrors.VehicleNotFound);

        if (manager.Enterprises.All(e => e.Id != vehicle.Enterprise.Id))
            return Result.Fail<RidesViewModel>(RidesHandlerErrors.ManagerNotAllowedToVehicle);

        // Эти объекты и так подгружаются автоматом
        List<Ride> rides = await DbContext.Rides
            .Where(r => r.Vehicle.Id == vehicle.Id
                        && r.StartTime >= query.StartTime.ToUniversalTime()
                        && r.EndTime <= query.EndTime.ToUniversalTime())
            .Include(r => r.Vehicle)
            .Include(r => r.StartPoint)
            //.ThenInclude(r => r.Location)
            .Include(r => r.EndPoint)
            //.ThenInclude(r => r.Location)
            .ToListAsync();

        TzInfo? enterpriseTimeZone = await DbContext.Enterprises
            .Where(e => e.Id == vehicle.Enterprise.Id)
            .Select(e => e.TimeZone)
            .FirstOrDefaultAsync();

        List<RideViewModel> rideViewModels = new List<RideViewModel>(rides.Count);

        foreach (Ride ride in rides)
        {
            GeoDetails? startGeoDetails = await _geoCodingService.GetGeoDetailsAsync(new GetGeoDatailsRequest
            {
                Latitude = ride.StartPoint.Location.Y,
                Longitude = ride.StartPoint.Location.X
            });

            GeoDetails? endGeoDetails = await _geoCodingService.GetGeoDetailsAsync(new GetGeoDatailsRequest
            {
                Latitude = ride.EndPoint.Location.Y,
                Longitude = ride.EndPoint.Location.X
            });

            DateTimeOffset startPointTime = _timeZoneService.ConvertToEnterpriseTimeZone(ride.StartPoint.Time, enterpriseTimeZone);
            DateTimeOffset endPointTime = _timeZoneService.ConvertToEnterpriseTimeZone(ride.EndPoint.Time, enterpriseTimeZone);

            RideViewModel rideViewModel = new RideViewModel
            {
                Id = ride.Id,
                VehicleId = ride.Vehicle.Id,
                StartTime = ride.StartTime,
                EndTime = ride.EndTime,
                StartGeoPoint = new RideViewModel.GeoPoint
                {
                    X = ride.StartPoint.Location.X,
                    Y = ride.StartPoint.Location.Y,
                    Address = startGeoDetails?.Address,
                    Time = startPointTime,
                },
                EndGeoPoint = new RideViewModel.GeoPoint
                {
                    X = ride.StartPoint.Location.X,
                    Y = ride.StartPoint.Location.Y,
                    Address = endGeoDetails?.Address,
                    Time = endPointTime,
                }
            };

            rideViewModels.Add(rideViewModel);
        }

        RidesViewModel ridesViewModel = new RidesViewModel
        {
            Rides = rideViewModels
        };

        return Result.Ok(ridesViewModel);
    }
}