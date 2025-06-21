namespace CarPark.Models;

public sealed class DriverVehicleAssignment
{
    public int DriverId { get; set; }

    public int VehicleId { get; set; }

    public bool IsActiveAssignment { get; set; }
}