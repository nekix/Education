using System.Text.Json.Serialization;

namespace CarPark.TrackGenerator.GraphHopper.Models;

public class RouteRequest
{
    [JsonPropertyName("points")]
    public required double[][] Points { get; init; }

    [JsonPropertyName("profile")]
    public required string Profile { get; init; }

    [JsonPropertyName("points_encoded")]
    public bool PointsEncoded { get; init; } = false;

    [JsonPropertyName("calc_points")]
    public bool CalcPoints { get; init; } = true;

    [JsonPropertyName("instructions")]
    public bool Instructions { get; init; } = false;

    [JsonPropertyName("elevation")]
    public bool Elevation { get; init; } = false;
}

