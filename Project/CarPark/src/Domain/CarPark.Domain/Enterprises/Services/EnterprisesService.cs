using CarPark.Enterprises.Errors;
using CarPark.Drivers;
using CarPark.Vehicles;
using FluentResults;
using static CarPark.Enterprises.Errors.EnterpriseDomainErrorCodes;

namespace CarPark.Enterprises.Services;

public sealed class EnterprisesService : IEnterprisesService
{
    public Result<Enterprise> CreateEnterprise(CreateEnterpriseRequest request)
    {
        EnterpriseCreateData data = new EnterpriseCreateData
        {
            Id = request.Id,
            Name = request.Name,
            LegalAddress = request.LegalAddress,
            TimeZone = request.TimeZone
        };

        return Enterprise.Create(data);
    }

    public Result UpdateEnterprise(Enterprise enterprise, UpdateEnterpriseRequest request)
    {
        EnterpriseUpdateData data = new EnterpriseUpdateData
        {
            Name = request.Name,
            LegalAddress = request.LegalAddress,
            TimeZone = request.TimeZone
        };

        return Enterprise.Update(enterprise, data);
    }

    public Result CheckCanDeleteEnterprise(Enterprise enterprise, IEnumerable<Vehicle> enterpriseVehicles, IEnumerable<Driver> enterpriseDrivers)
    {
        if (enterprise.Managers.Count != 0)
            return Result.Fail(new EnterpriseDomainError(EnterpriseHasOtherManagersError));

        if (enterpriseVehicles.Any())
            return Result.Fail(new EnterpriseDomainError(EnterpriseHasVehiclesError));

        if (enterpriseDrivers.Any())
            return Result.Fail(new EnterpriseDomainError(EnterpriseHasDriversError));

        return Result.Ok();
    }
}
