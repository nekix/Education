namespace CarPark.Application.Geo.GeoCoding;

public class GetGeoDatailsRequest
{
    public required double Latitude { get; init;  }

    public required double Longitude { get; init;  }
}