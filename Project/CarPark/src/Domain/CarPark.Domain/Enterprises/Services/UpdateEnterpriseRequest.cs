using CarPark.TimeZones;

namespace CarPark.Enterprises.Services;

public record UpdateEnterpriseRequest
{
    public required string Name { get; init; }

    public required string LegalAddress { get; init; }

    public required TzInfo? TimeZone { get; init; }
}
