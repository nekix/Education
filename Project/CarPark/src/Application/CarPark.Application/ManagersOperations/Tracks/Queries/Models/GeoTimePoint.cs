namespace CarPark.ManagersOperations.Tracks.Queries.Models;

public class GeoTimePoint
{
    public required DateTimeOffset Time { get; set; }

    public required double X { get; set; }

    public required double Y { get; set; }
}