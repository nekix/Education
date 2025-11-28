using CarPark.Models;
using CarPark.Enterprises;
using CarPark.Drivers;

namespace CarPark.Vehicles.Services;

public record CreateVehicleRequest
{
    public required Guid Id { get; init; }
    public required Model Model { get; init; }
    public required Enterprise Enterprise { get; init; }
    public required string VinNumber { get; init; }
    public required decimal Price { get; init; }
    public required int ManufactureYear { get; init; }
    public required int Mileage { get; init; }
    public required string Color { get; init; }
    public required List<Driver> AssignedDrivers { get; init; }
    public required Driver? ActiveAssignedDriver { get; init; }
    public required DateTimeOffset AddedToEnterpriseAt { get; init; }
}