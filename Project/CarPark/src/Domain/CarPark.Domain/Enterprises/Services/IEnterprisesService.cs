using CarPark.Managers;
using CarPark.Drivers;
using CarPark.Vehicles;
using FluentResults;

namespace CarPark.Enterprises.Services;

public interface IEnterprisesService
{
    Result<Enterprise> CreateEnterprise(CreateEnterpriseRequest request);

    Result UpdateEnterprise(Enterprise enterprise, UpdateEnterpriseRequest request);

    Result CheckCanDeleteEnterprise(Enterprise enterprise, IEnumerable<Vehicle> enterpriseVehicles, IEnumerable<Driver> enterpriseDrivers);
}
