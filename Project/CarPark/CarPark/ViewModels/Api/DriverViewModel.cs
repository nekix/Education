namespace CarPark.ViewModels.Api;

public class DriverViewModel
{
    public required int Id { get; set; }

    public required int EnterpriseId { get; set; }

    public required string FullName { get; set; }

    public required string DriverLicenseNumber { get; set; }

    public required List<VehicleAssignmentViewModel> VehiclesAssignments { get; set; }
}

public class VehicleAssignmentViewModel
{
    public required int VehicleId { get; set; }

    public required bool IsActive { get; set; }
}