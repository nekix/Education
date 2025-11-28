using CarPark.Vehicles;
using FluentResults;

namespace CarPark.Drivers.Services;

public interface IDriversService
{
    Result<Driver> CreateDriver(CreateDriverRequest request);

    Result UpdateDriver(Driver driver, UpdateDriverRequest request);

    Result CheckCanDeleteDriver(Driver driver, IEnumerable<Vehicle> assignedVehicles);
}