namespace CarPark.TrackGenerator.Models;

public class BulkRideGenerationOptions
{
    public required DateTimeOffset StartDate { get; init; }
    public required DateTimeOffset EndDate { get; init; }
    public required double ActiveDaysRatio { get; init; }
    public required int AverageRidesPerDay { get; init; }
    public required int MinRideDurationMinutes { get; init; }
    public required int MaxRideDurationMinutes { get; init; }
    public required string ConnectionString { get; init; }
    public required string GraphHopperApiKey { get; init; }
}