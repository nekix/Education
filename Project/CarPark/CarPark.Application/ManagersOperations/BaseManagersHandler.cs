using CarPark.Data;
using CarPark.Managers;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CarPark.ManagersOperations;

public abstract class BaseManagersHandler
{
    protected readonly ApplicationDbContext DbContext;

    protected BaseManagersHandler(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
    }

    protected async Task<Result<Manager>> GetManagerAsync(Guid managerId, bool efTracking)
    {
        Manager? manager = await DbContext.Managers
            .Include(e => e.Enterprises)
            .FirstOrDefaultAsync(m => m.Id == managerId);

        return manager != null
            ? Result.Ok(manager)
            : Result.Fail<Manager>(ManagersOperationsErrors.ManagerNotExist);
    }
}