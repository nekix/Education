namespace CarPark.TimeZones.Conversion;

public class TimeZoneDisplayInfo
{
    public required string DisplayName { get; set; }
    public required TimeSpan Offset { get; set; }
    public required string IanaId { get; set; }
}