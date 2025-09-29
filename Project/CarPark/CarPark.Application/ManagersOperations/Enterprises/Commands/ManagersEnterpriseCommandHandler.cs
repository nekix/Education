using CarPark.Data;
using CarPark.Enterprises;
using CarPark.Managers;
using CarPark.Shared.CQ;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CarPark.ManagersOperations.Enterprises.Commands;

public class ManagersEnterpriseCommandHandler : BaseManagersHandler,
    ICommandHandler<DeleteEnterpriseCommand, Result>
{
    public ManagersEnterpriseCommandHandler(ApplicationDbContext dbContext) : base(dbContext)
    {
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

        if (enterprise.Managers.Any(m => m.Id != manager.Id))
            return Result.Fail(EnterprisesHandlersErrors.ForbidDeleteEnterpriseWithOtherManagers);

        bool hasAnyVehicles = await DbContext.Vehicles.AnyAsync(v => v.Enterprise.Id == enterprise.Id);
        if (hasAnyVehicles)
            return Result.Fail(EnterprisesHandlersErrors.ForbidDeleteWithAnyVehicles);

        bool hasAnyDrivers = await DbContext.Drivers.AnyAsync(d => d.EnterpriseId == enterprise.Id);
        if (hasAnyDrivers)
            return Result.Fail(EnterprisesHandlersErrors.ForbidDeleteWithAnyDrivers);

        return await RemoveEnterpriseAsync(enterprise);
    }

    private async Task<Result> RemoveEnterpriseAsync(Enterprise enterprise)
    {
        DbContext.Enterprises.Remove(enterprise);
        await DbContext.SaveChangesAsync();

        return Result.Ok();
    }
}