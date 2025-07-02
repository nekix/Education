namespace CarPark.Models;

public sealed class Vehicle
{
    public int Id { get; set; }

    public int ModelId { get; set; }

    public int EnterpriseId { get; set; }

    public string VinNumber { get; set; }

    public decimal Price { get; set; }

    public int ManufactureYear { get; set; }

    public int Mileage { get; set; }

    public string Color { get; set; }

    public List<Driver> AssignedDrivers { get; set; } = new List<Driver>();

    public Driver? ActiveAssignedDriver { get; set; }
}