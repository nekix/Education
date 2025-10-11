namespace CarPark.ManagersOperations.Rides.Queries;

public class RideViewModel
{
    public required Guid Id { get; set; }

    public required Guid VehicleId { get; set; }

    public required DateTimeOffset StartTime { get; set; }

    public required DateTimeOffset EndTime { get; set; }

    public required GeoPoint StartGeoPoint { get; set; }

    public required GeoPoint EndGeoPoint { get; set; }

    public class GeoPoint
    {
        public required DateTimeOffset Time { get; set; }

        public required double X { get; set; }

        public required double Y { get; set; }

        public required string? Address { get; set; }
    }
}