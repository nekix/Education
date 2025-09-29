using CarPark.Data;
using CarPark.Managers;
using CarPark.ManagersOperations.Vehicles.Queries.Models;
using CarPark.Services.TimeZones;
using CarPark.Shared.CQ;
using CarPark.TimeZones;
using CarPark.Vehicles;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CarPark.ManagersOperations.Vehicles.Queries;

public class ManagersVehiclesQueryHandler : BaseManagersHandler,
    IQueryHandler<GetVehicleQuery, Result<VehicleDto>>,
    IQueryHandler<GetVehiclesListQuery, Result<PaginatedVehicles>>
        
{
    private readonly ITimeZoneConversionService _timeZoneService;

    public ManagersVehiclesQueryHandler(ApplicationDbContext context,
        ITimeZoneConversionService timeZoneService) : base(context)
    {
        _timeZoneService = timeZoneService;
    }

    public async Task<Result<VehicleDto>> Handle(GetVehicleQuery query)
    {
        Result<Manager> getManager = await GetManagerAsync(query.RequestingManagerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult<VehicleDto>();

        Manager manager = getManager.Value;

        IQueryable<Vehicle> dbQuery = DbContext.Vehicles
            .Where(v => v.Id == query.VehicleId);

        IQueryable<VehicleDto> viewModelQuery = TransformToViewModelQuery(dbQuery);

        VehicleDto? vehicleViewModel = await viewModelQuery.FirstOrDefaultAsync();
        if (vehicleViewModel == null)
            return Result.Fail(VehiclesHandlersErrors.VehicleNotExist);

        if (manager.Enterprises.All(v => v.Id != vehicleViewModel.EnterpriseId))
            return Result.Fail(VehiclesHandlersErrors.ManagerNotAllowedToVehicle);

        // Get enterprise timezone info
        TzInfo? enterpriseTimeZone = await DbContext.Enterprises
            .Where(e => e.Id == vehicleViewModel.EnterpriseId)
            .Select(e => e.TimeZone)
            .FirstOrDefaultAsync();

        // Convert date to enterprise timezone
        vehicleViewModel.AddedToEnterpriseAt = _timeZoneService.ConvertToEnterpriseTimeZone(vehicleViewModel.AddedToEnterpriseAt, enterpriseTimeZone);

        return Result.Ok(vehicleViewModel);
    }

    public async Task<Result<PaginatedVehicles>> Handle(GetVehiclesListQuery query)
    {
        Result<Manager> getManager = await GetManagerAsync(query.RequestingManagerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult<PaginatedVehicles>();

        Manager manager = getManager.Value;

        IQueryable<Vehicle> dbQuery = DbContext.Vehicles
            .Where(v => v.Enterprise.Managers.Any(m => m.Id == manager.Id))
            .OrderBy(v => v.Enterprise.Id)
            .ThenBy(v => v.Color)
            .ThenBy(v => v.VinNumber);

        uint total = (uint)dbQuery.Count();

        IQueryable<Vehicle> paginatedQuery = dbQuery
            .Skip((int)query.Offset)
            .Take((int)query.Limit);

        IQueryable<VehicleDto> viewModelQuery = TransformToViewModelQuery(paginatedQuery);

        List<VehicleDto> viewModels = await viewModelQuery
            .ToListAsync();

        // Get enterprises for timezone info
        Dictionary<int, TzInfo?> enterpriseTimeZones = await DbContext.Enterprises
            .Where(e => viewModels.Select(v => v.EnterpriseId).Contains(e.Id))
            .Select(e => new { e.Id, e.TimeZone })
            .ToDictionaryAsync(e => e.Id, e => e.TimeZone);

        // Convert dates to enterprise timezone
        foreach (VehicleDto model in viewModels)
        {
            if (enterpriseTimeZones.TryGetValue(model.EnterpriseId, out TzInfo? timeZoneInfo))
            {
                model.AddedToEnterpriseAt = _timeZoneService.ConvertToEnterpriseTimeZone(
                    model.AddedToEnterpriseAt,
                    timeZoneInfo);
            }
        }

        PaginatedVehicles paginatedVehicles = new PaginatedVehicles
        {
            Data = viewModels,
            Meta = new PaginatedVehicles.Metadata
            {
                Limit = query.Limit,
                Offset = query.Offset,
                Total = total
            }
        };

        return Result.Ok(paginatedVehicles);
    }

    private static IQueryable<VehicleDto> TransformToViewModelQuery(IQueryable<Vehicle> query)
    {
        IQueryable<VehicleDto> vehiclesQuery =
            from v in query
            from d in v.AssignedDrivers.DefaultIfEmpty()
            group d by new
            {
                v.Id,
                ModelId = v.Model.Id,
                EnterpriseId = v.Enterprise.Id,
                v.VinNumber,
                v.Price,
                v.ManufactureYear,
                v.Mileage,
                v.Color,
                v.AddedToEnterpriseAt,
                ActiveDriverId = v.ActiveAssignedDriver != null ? v.ActiveAssignedDriver.Id : (int?)null
            } into g
            select new VehicleDto
            {
                Id = g.Key.Id,
                ModelId = g.Key.ModelId,
                EnterpriseId = g.Key.EnterpriseId,
                VinNumber = g.Key.VinNumber,
                Price = g.Key.Price,
                ManufactureYear = g.Key.ManufactureYear,
                Mileage = g.Key.Mileage,
                Color = g.Key.Color,
                AddedToEnterpriseAt = g.Key.AddedToEnterpriseAt,
                DriversAssignments = new VehicleDto.DriversAssignmentsViewModel
                {
                    DriversIds = EF.Functions.ArrayAgg(
                        g.Where(x => x != null)
                            .Select(x => x.Id)
                            .Order()),
                    ActiveDriverId = g.Key.ActiveDriverId
                }
            };

        return vehiclesQuery;
    }
}