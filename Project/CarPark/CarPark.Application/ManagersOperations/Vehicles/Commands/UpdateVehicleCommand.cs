using FluentResults;
using CarPark.Shared.CQ;

namespace CarPark.ManagersOperations.Vehicles.Commands;

public class UpdateVehicleCommand : BaseManagerCommandQuery, ICommand<Result<Guid>>
{
    public required Guid VehicleId { get; set; }

    public required Guid ModelId { get; set; }

    public required Guid EnterpriseId { get; set; }

    public required string VinNumber { get; set; }

    public required decimal Price { get; set; }

    public required int ManufactureYear { get; set; }

    public required int Mileage { get; set; }

    public required string Color { get; set; }

    public required List<Guid> DriverIds { get; set; }

    public required Guid? ActiveDriverId { get; set; }

    public required DateTimeOffset AddedToEnterpriseAt { get; set; }
}