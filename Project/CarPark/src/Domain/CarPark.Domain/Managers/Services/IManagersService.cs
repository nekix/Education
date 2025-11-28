using FluentResults;

namespace CarPark.Managers.Services;

public interface IManagersService
{
    Result<Manager> CreateManager(CreateManagerRequest request);
}