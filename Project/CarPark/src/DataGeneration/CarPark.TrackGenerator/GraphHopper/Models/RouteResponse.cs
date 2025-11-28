using System.Text.Json.Serialization;

namespace CarPark.TrackGenerator.GraphHopper.Models;

public class RouteResponse
{
    [JsonPropertyName("paths")]
    public RoutePath[] Paths { get; init; } = [];

    [JsonPropertyName("info")]
    public ResponseInfo? Info { get; init; }

    [JsonPropertyName("hints")]
    public ResponseHints? Hints { get; init; }
}

public class RoutePath
{
    [JsonPropertyName("distance")]
    public double Distance { get; init; } // meters

    [JsonPropertyName("time")]
    public long Time { get; init; } // milliseconds

    [JsonPropertyName("weight")]
    public double? Weight { get; init; }

    [JsonPropertyName("points")]
    public GeoJsonLineString? Points { get; init; }

    [JsonPropertyName("bbox")]
    public double[]? BoundingBox { get; init; }

    [JsonPropertyName("ascend")]
    public double? Ascend { get; init; }

    [JsonPropertyName("descend")]
    public double? Descend { get; init; }

    [JsonPropertyName("snapped_waypoints")]
    public GeoJsonLineString? SnappedWaypoints { get; init; }
}

public class GeoJsonLineString
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "LineString";

    [JsonPropertyName("coordinates")]
    public double[][] Coordinates { get; init; } = [];
}

public class ResponseInfo
{
    [JsonPropertyName("copyrights")]
    public string[]? Copyrights { get; init; }

    [JsonPropertyName("took")]
    public int? Took { get; init; } // milliseconds

    [JsonPropertyName("road_data_timestamp")]
    public string? RoadDataTimestamp { get; init; }
}

public class ResponseHints
{
    [JsonPropertyName("visited_nodes.sum")]
    public int? VisitedNodesSum { get; init; }

    [JsonPropertyName("visited_nodes.average")]
    public double? VisitedNodesAverage { get; init; }
}

