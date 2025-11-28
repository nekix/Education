using CarPark.TimeZones;

namespace CarPark.Enterprises;

internal sealed record EnterpriseUpdateData
{
    public required string Name { get; init; }

    public required string LegalAddress { get; init; }

    public required TzInfo? TimeZone { get; init; }
}
