using CarPark.Data;
using CarPark.Drivers;
using CarPark.Managers;
using CarPark.ManagersOperations.Drivers.Queries.Models;
using CarPark.ManagersOperations.Vehicles;
using CarPark.Shared.CQ;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CarPark.ManagersOperations.Drivers.Queries;

public class ManagersDriverQueryHandler : BaseManagersHandler,
    IQueryHandler<GetDriverQuery, Result<DriverDto>>,
    IQueryHandler<GetDriversListQuery, Result<PaginatedDrivers>>
{
    public ManagersDriverQueryHandler(ApplicationDbContext dbContext) : base(dbContext)
    {

    }

    public async Task<Result<DriverDto>> Handle(GetDriverQuery query)
    {
        Result<Manager> getManager = await GetManagerAsync(query.RequestingManagerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult<DriverDto>();

        Manager manager = getManager.Value;

        IQueryable<Driver> dbQuery = DbContext.Drivers
            .Where(d => d.Id == query.DriverId);

        IQueryable<DriverDto> viewModelQuery = TransformToViewModelQuery(dbQuery);

        DriverDto? driverViewModel = await viewModelQuery.FirstOrDefaultAsync();
        if (driverViewModel == null)
            return Result.Fail<DriverDto>(DriversHandlerErrors.DriverNotExist);

        if (manager.Enterprises.All(v => v.Id != driverViewModel.EnterpriseId))
            return Result.Fail(DriversHandlerErrors.ManagerNotAllowedToVehicle);

        return Result.Ok(driverViewModel);
    }

    public async Task<Result<PaginatedDrivers>> Handle(GetDriversListQuery query)
    {
        Result<Manager> getManager = await GetManagerAsync(query.RequestingManagerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult<PaginatedDrivers>();

        Manager manager = getManager.Value;

        List<int> enterprisesIds = manager.Enterprises.Select(e => e.Id).ToList();

        IQueryable<Driver> dbQuery = DbContext.Drivers
            .Where(v => enterprisesIds.Contains(v.EnterpriseId))
            .OrderBy(v => v.EnterpriseId)
            .ThenBy(v => v.FullName);

        uint total = (uint)dbQuery.Count();

        IQueryable<Driver> paginatedQuery = dbQuery
            .Skip((int)query.Offset)
            .Take((int)query.Limit);

        IQueryable<DriverDto> viewModelQuery = TransformToViewModelQuery(paginatedQuery);

        List<DriverDto> viewModels = await viewModelQuery
            .ToListAsync();

        PaginatedDrivers paginatedVehicles = new PaginatedDrivers
        {
            Data = viewModels,
            Meta = new PaginatedDrivers.Metadata
            {
                Limit = query.Limit,
                Offset = query.Offset,
                Total = total
            }
        };

        return Result.Ok(paginatedVehicles);
    }

    private static IQueryable<DriverDto> TransformToViewModelQuery(IQueryable<Driver> query)
    {
        IQueryable<DriverDto> viewModelQuery =
            from d in query
            from v in d.AssignedVehicles.DefaultIfEmpty()
            group v by new
            {
                d.Id,
                d.EnterpriseId,
                d.FullName,
                d.DriverLicenseNumber,
                ActiveVehicleId = d.ActiveAssignedVehicle != null ? d.ActiveAssignedVehicle.Id : (int?)null
            } into g
            select new DriverDto
            {
                Id = g.Key.Id,
                EnterpriseId = g.Key.EnterpriseId,
                FullName = g.Key.FullName,
                DriverLicenseNumber = g.Key.DriverLicenseNumber,
                VehiclesAssignments = new DriverDto.VehiclesAssignmentsViewModel
                {
                    VehiclesIds = EF.Functions.ArrayAgg(
                        g.Where(x => x != null)
                            .Select(x => x.Id)
                            .Order()),
                    ActiveVehicleId = g.Key.ActiveVehicleId,
                }
            };

        return viewModelQuery;
    }
}