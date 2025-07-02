namespace CarPark.Models;

public sealed class Driver
{
    public int Id { get; set; }

    public int EnterpriseId { get; set; }

    public string FullName { get; set; }

    public string DriverLicenseNumber { get; set; }

    public List<Vehicle> AssignedVehicles { get; set; } = new List<Vehicle>();

    public Vehicle? ActiveAssignedVehicle { get; set; }
}