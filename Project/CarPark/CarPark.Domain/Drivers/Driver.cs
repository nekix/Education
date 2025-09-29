using CarPark.Vehicles;

namespace CarPark.Drivers;

public sealed class Driver
{
    public required int Id { get; set; }

    public required int EnterpriseId { get; set; }

    public required string FullName { get; set; }

    public required string DriverLicenseNumber { get; set; }

    public required List<Vehicle> AssignedVehicles { get; set; }

    public required Vehicle? ActiveAssignedVehicle { get; set; }
}