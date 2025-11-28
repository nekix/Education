namespace CarPark.Drivers;

internal sealed record DriverUpdateData
{
    public required Guid EnterpriseId { get; init; }

    public required string FullName { get; init; }

    public required string DriverLicenseNumber { get; init; }
}