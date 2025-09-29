using CarPark.Data;
using CarPark.Managers;
using CarPark.Shared.CQ;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CarPark.ManagersOperations.Enterprises.Queries;

public class ManagersEnterprisesQueryHandler : BaseManagersHandler,
    IQueryHandler<GetEnterpriseQuery, Result<EnterpriseDto>>,
    IQueryHandler<GetEnterprisesCollectionQuery, Result<List<EnterpriseDto>>>
{
    public ManagersEnterprisesQueryHandler(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Result<EnterpriseDto>> Handle(GetEnterpriseQuery query)
    {
        Result<Manager> getManager = await GetManagerAsync(query.RequestingManagerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult<EnterpriseDto>();

        Manager manager = getManager.Value;

        if (manager.Enterprises.All(e => e.Id != query.EnterpriseId))
            return Result.Fail<EnterpriseDto>(EnterprisesHandlersErrors.ManagerNotAllowedToEnterprise);

        var driversQuery =
            from d in DbContext.Drivers
            where d.EnterpriseId == query.EnterpriseId
            select new
            {
                EnterpriseId = d.EnterpriseId,
                Id = d.Id
            };

        var vehiclesQuery =
            from v in DbContext.Vehicles
            where v.Enterprise.Id == query.EnterpriseId
            select new
            {
                EnterpriseId = v.Enterprise.Id,
                Id = v.Id
            };

        IQueryable<EnterpriseDto> dtoQuery = from e in DbContext.Enterprises
            let drivers = driversQuery
                .Select(d => d.Id)
                .OrderBy(id => id)
                .ToList()
            let vehicles = vehiclesQuery
                .Select(v => v.Id)
                .OrderBy(id => id)
                .ToList()
            where e.Id == query.EnterpriseId
            select new EnterpriseDto
            {
                Id = e.Id,
                Name = e.Name,
                LegalAddress = e.LegalAddress,
                TimeZoneId = e.TimeZone == null ? null : e.TimeZone.Id,
                RelatedEntities = new EnterpriseDto.RelatedEntitiesDto
                {
                    DriversIds = drivers.ToList(),
                    VehiclesIds = vehicles.ToList()
                }
            };

        EnterpriseDto dto = await dtoQuery.FirstAsync();

        return Result.Ok<EnterpriseDto>(dto);
    }

    public async Task<Result<List<EnterpriseDto>>> Handle(GetEnterprisesCollectionQuery query)
    {
        Result<Manager> getManager = await GetManagerAsync(query.RequestingManagerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult<List<EnterpriseDto>>();

        Manager manager = getManager.Value;

        List<int> enterprisesIds = manager.Enterprises.Select(e => e.Id).ToList();

        var driversQuery =
            from d in DbContext.Drivers
            where enterprisesIds.Contains(d.EnterpriseId) 
            select new
            {
                EnterpriseId = d.EnterpriseId,
                Id = d.Id
            };

        var vehiclesQuery =
            from v in DbContext.Vehicles
            where enterprisesIds.Contains(v.Enterprise.Id)
            select new
            {
                EnterpriseId = v.Enterprise.Id,
                Id = v.Id
            };

        IQueryable<EnterpriseDto> dtoQuery = from e in DbContext.Enterprises
            let drivers = driversQuery
                .Where(d => d.EnterpriseId == e.Id)
                .Select(d => d.Id)
                .OrderBy(id => id)
                .ToList()
            let vehicles = vehiclesQuery
                .Where(v => v.EnterpriseId == e.Id)
                .Select(v => v.Id)
                .OrderBy(id => id)
                .ToList()
            where enterprisesIds.Contains(e.Id)
            orderby e.Id
            select new EnterpriseDto
            {
                Id = e.Id,
                Name = e.Name,
                LegalAddress = e.LegalAddress,
                TimeZoneId = e.TimeZone == null ? null : e.TimeZone.Id,
                RelatedEntities = new EnterpriseDto.RelatedEntitiesDto
                {
                    DriversIds = drivers,
                    VehiclesIds = vehicles
                }
            };

        List<EnterpriseDto> dtoCollection = await dtoQuery.ToListAsync();

        return Result.Ok<List<EnterpriseDto>>(dtoCollection);
    }
}