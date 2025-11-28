using CarPark.CQ;
using CarPark.Data;
using CarPark.Drivers;
using CarPark.Enterprises;
using CarPark.Enterprises.Errors;
using CarPark.Enterprises.Services;
using CarPark.Managers;
using CarPark.Vehicles;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CarPark.ManagersOperations.Enterprises.Commands;

public class ManagersEnterpriseCommandHandler : BaseManagersHandler,
    ICommandHandler<DeleteEnterpriseCommand, Result>
{
    private readonly IEnterprisesService _enterprisesService;

    public ManagersEnterpriseCommandHandler(ApplicationDbContext dbContext, IEnterprisesService enterprisesService) : base(dbContext)
    {
        _enterprisesService = enterprisesService;
    }

    public async Task<Result> Handle(DeleteEnterpriseCommand command)
    {
        Result<Manager> getManager = await GetManagerAsync(command.RequestingManagerId, false);
        if (getManager.IsFailed)
            return getManager.ToResult();

        Manager manager = getManager.Value;

        Enterprise? enterprise = await DbContext.Enterprises
            .Include(e => e.Managers)
            .Where(e => e.Id == command.EnterpriseId)
            .FirstOrDefaultAsync();

        if (enterprise == null)
            return Result.Fail(EnterprisesHandlersErrors.EnterpriseNotExist);

        if (manager.Enterprises.All(e => e.Id != enterprise.Id))
            return Result.Fail(EnterprisesHandlersErrors.ManagerNotAllowedToEnterprise);

        // Remove current manager from enterprise before domain validation
        // This is necessary because enterprise can only be deleted if it has no managers
        enterprise.Managers.Remove(manager);

        List<Vehicle> enterpriseVehicles = await DbContext.Vehicles
            .Where(v => v.Enterprise.Id == enterprise.Id)
            .ToListAsync();

        List<Driver> enterpriseDrivers = await DbContext.Drivers
            .Where(d => d.EnterpriseId == enterprise.Id)
            .ToListAsync();

        // Use domain service to check business invariants
        Result checkCanDeleteResult = _enterprisesService.CheckCanDeleteEnterprise(enterprise, enterpriseVehicles, enterpriseDrivers);
        if (checkCanDeleteResult.IsFailed)
        {
            IEnumerable<IError> errors = checkCanDeleteResult.Errors
                .Select(e => e is EnterpriseDomainError domainError ? EnterprisesErrors.MapDomainError(domainError) : e);
            return Result.Fail(errors);
        }

        DbContext.Enterprises.Remove(enterprise);
        await DbContext.SaveChangesAsync();

        return Result.Ok();
    }
}