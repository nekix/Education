namespace CarPark.ManagersOperations.Drivers.Queries.Models;

public class DriverDto
{
    public required Guid Id { get; set; }

    public required Guid EnterpriseId { get; set; }

    public required string FullName { get; set; }

    public required string DriverLicenseNumber { get; set; }

    public required VehiclesAssignmentsViewModel VehiclesAssignments { get; set; }

    public class VehiclesAssignmentsViewModel
    {
        public required IEnumerable<Guid> VehiclesIds { get; set; }

        public required Guid? ActiveVehicleId { get; set; }
    }
}