namespace CarPark.ManagersOperations.Vehicles.Queries.Models;

public class VehicleDto
{
    public required Guid Id { get; set; }

    public required Guid ModelId { get; set; }

    public required Guid EnterpriseId { get; set; }

    public required string VinNumber { get; set; }

    public required decimal Price { get; set; }

    public required int ManufactureYear { get; set; }

    public required int Mileage { get; set; }

    public required string Color { get; set; }

    public required DriversAssignmentsViewModel DriversAssignments { get; set; }

    public required DateTimeOffset AddedToEnterpriseAt { get; set; }

    public class DriversAssignmentsViewModel
    {
        public required IEnumerable<Guid> DriversIds { get; set; }

        public required Guid? ActiveDriverId { get; set; }
    }
}