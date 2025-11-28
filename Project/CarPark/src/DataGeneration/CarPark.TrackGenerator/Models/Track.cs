namespace CarPark.TrackGenerator.Models;

public class Track
{
    public Guid VehicleId { get; init; }
    public IReadOnlyList<GeoTimePoint> Points { get; init; } = new List<GeoTimePoint>();
}
