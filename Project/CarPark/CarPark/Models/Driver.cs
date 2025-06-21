namespace CarPark.Models;

public sealed class Driver
{
    public int Id { get; set; }

    public int EnterpriseId { get; set; }

    public string FullName { get; set; }

    public string DriverLicenseNumber { get; set; }
}