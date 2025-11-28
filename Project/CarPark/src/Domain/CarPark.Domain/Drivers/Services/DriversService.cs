using CarPark.Drivers.Errors;
using CarPark.Vehicles;
using FluentResults;
using static CarPark.Drivers.Errors.DriverDomainErrorCodes;

namespace CarPark.Drivers.Services;

public sealed class DriversService : IDriversService
{
    public Result<Driver> CreateDriver(CreateDriverRequest request)
    {
        DriverCreateData data = new DriverCreateData
        {
            Id = request.Id,
            EnterpriseId = request.EnterpriseId,
            FullName = request.FullName,
            DriverLicenseNumber = request.DriverLicenseNumber
        };

        return Driver.Create(data);
    }

    public Result UpdateDriver(Driver driver, UpdateDriverRequest request)
    {
        DriverUpdateData data = new DriverUpdateData
        {
            EnterpriseId = request.EnterpriseId,
            FullName = request.FullName,
            DriverLicenseNumber = request.DriverLicenseNumber
        };

        return Driver.Update(driver, data);
    }

    public Result CheckCanDeleteDriver(Driver driver, IEnumerable<Vehicle> assignedVehicles)
    {
        if (assignedVehicles.Any())
            return Result.Fail(new DriverDomainError(DriverHasAssignedVehicles));

        return Result.Ok();
    }
}