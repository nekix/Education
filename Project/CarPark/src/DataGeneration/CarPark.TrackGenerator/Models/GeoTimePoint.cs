using NetTopologySuite.Geometries;

namespace CarPark.TrackGenerator.Models;

public class GeoTimePoint
{
    public required Point Location { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public double SpeedKmH { get; init; }
    public double AccelerationKmH2 { get; init; }
}
