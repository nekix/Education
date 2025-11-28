using CarPark.TimeZones;

namespace CarPark.Enterprises;

internal sealed record EnterpriseCreateData
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string LegalAddress { get; init; }

    public required TzInfo? TimeZone { get; init; }
}
