namespace CarPark.ViewModels.Api;

public class VehicleViewModel
{
    public required int Id { get; set; }

    public required int ModelId { get; set; }

    public required int EnterpriseId { get; set; }

    public required string VinNumber { get; set; }

    public required decimal Price { get; set; }

    public required int ManufactureYear { get; set; }

    public required int Mileage { get; set; }

    public required string Color { get; set; }

    public required DriversAssignmentsViewModel DriversAssignments { get; set; }

    public class DriversAssignmentsViewModel
    {
        public required IEnumerable<int> DriversIds { get; set; }

        public required int? ActiveDriverId { get; set; }
    }
}