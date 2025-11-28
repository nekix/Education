using CarPark.Vehicles;
using FluentResults;

namespace CarPark.Drivers;

public sealed class Driver
{
    public Guid Id { get; private set; }

    public Guid EnterpriseId { get; private set; }

    public string FullName { get; private set; }

    public string DriverLicenseNumber { get; private set; }

    public List<Vehicle> AssignedVehicles { get; private set; }

    public Vehicle? ActiveAssignedVehicle { get; private set; }

    #pragma warning disable CS8618
    [Obsolete("Only for ORM and deserialization! Do not use!")]
    private Driver()
    {
        // Only for ORM and deserialization! Do not use!
    }
    #pragma warning restore CS8618

    private Driver(
        Guid id,
        Guid enterpriseId,
        string fullName,
        string driverLicenseNumber)
    {
        Id = id;
        EnterpriseId = enterpriseId;
        FullName = fullName;
        DriverLicenseNumber = driverLicenseNumber;
        AssignedVehicles = new List<Vehicle>();
        ActiveAssignedVehicle = null;
    }

    internal static Result<Driver> Create(DriverCreateData data)
    {
        Driver driver = new Driver(
            data.Id,
            data.EnterpriseId,
            data.FullName,
            data.DriverLicenseNumber);

        return Result.Ok(driver);
    }

    internal static Result Update(Driver driver, DriverUpdateData data)
    {
        driver.EnterpriseId = data.EnterpriseId;
        driver.FullName = data.FullName;
        driver.DriverLicenseNumber = data.DriverLicenseNumber;

        return Result.Ok();
    }
}