using CarPark.Managers.Errors;
using FluentResults;

namespace CarPark.Managers.Services;

public sealed class ManagersService : IManagersService
{
    public Result<Manager> CreateManager(CreateManagerRequest request)
    {
        ManagerCreateData data = new ManagerCreateData
        {
            Id = request.Id,
            IdentityUserId = request.IdentityUserId,
            Enterprises = request.Enterprises
        };

        return Manager.Create(data);
    }
}