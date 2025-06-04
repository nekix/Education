namespace CarPark.Models;

public sealed class Vehicle
{
    public int Id { get; set; }

    public string VinNumber { get; set; }

    public decimal Price { get; set; }

    public int ManufactureYear { get; set; }

    public int Mileage { get; set; }

    public string Color { get; set; }
}