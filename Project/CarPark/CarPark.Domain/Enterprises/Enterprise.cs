using CarPark.Managers;
using CarPark.TimeZones;

namespace CarPark.Enterprises;

public sealed class Enterprise
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required string LegalAddress { get; set; }

    public required List<Manager> Managers { get; set; }

    public required TzInfo? TimeZone { get; set; }
}