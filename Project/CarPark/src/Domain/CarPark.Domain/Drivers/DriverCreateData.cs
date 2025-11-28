namespace CarPark.Drivers;

internal sealed record DriverCreateData
{
    public required Guid Id { get; init; }

    public required Guid EnterpriseId { get; init; }

    public required string FullName { get; init; }

    public required string DriverLicenseNumber { get; init; }
}