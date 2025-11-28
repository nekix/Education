namespace CarPark.Drivers.Services;

public record CreateDriverRequest
{
    public required Guid Id { get; init; }

    public required Guid EnterpriseId { get; init; }

    public required string FullName { get; init; }

    public required string DriverLicenseNumber { get; init; }
}