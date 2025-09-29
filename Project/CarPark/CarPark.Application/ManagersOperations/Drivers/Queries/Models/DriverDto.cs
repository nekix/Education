namespace CarPark.ManagersOperations.Drivers.Queries.Models;

public class DriverDto
{
    public required int Id { get; set; }

    public required int EnterpriseId { get; set; }

    public required string FullName { get; set; }

    public required string DriverLicenseNumber { get; set; }

    public required VehiclesAssignmentsViewModel VehiclesAssignments { get; set; }

    public class VehiclesAssignmentsViewModel
    {
        public required IEnumerable<int> VehiclesIds { get; set; }

        public required int? ActiveVehicleId { get; set; }
    }
}