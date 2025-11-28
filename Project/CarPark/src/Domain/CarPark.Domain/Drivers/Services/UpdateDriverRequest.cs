namespace CarPark.Drivers.Services;

public record UpdateDriverRequest
{
    public required Guid EnterpriseId { get; init; }

    public required string FullName { get; init; }

    public required string DriverLicenseNumber { get; init; }
}