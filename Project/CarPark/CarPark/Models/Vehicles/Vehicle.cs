using CarPark.Models.Drivers;

namespace CarPark.Models.Vehicles;

public sealed class Vehicle
{
    public required int Id { get; set; }

    public required int ModelId { get; set; }

    public required int EnterpriseId { get; set; }

    public required string VinNumber { get; set; }

    public required decimal Price { get; set; }

    public required int ManufactureYear { get; set; }

    public required int Mileage { get; set; }

    public required string Color { get; set; }

    public required List<Driver> AssignedDrivers { get; set; }

    public required Driver? ActiveAssignedDriver { get; set; }
}