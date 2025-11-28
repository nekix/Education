using NetTopologySuite.Geometries;

namespace CarPark.TrackGenerator.Interfaces;

public interface IRouteGenerationService
{
    Task<LineString> GenerateRouteAsync(RouteGenerationOptions options);
    Task<LineString> GenerateRouteFromPointAsync(Point startPoint, RouteGenerationOptions options);
}

public class RouteGenerationOptions
{
    // Центр области
    public required Point CenterPoint { get; init; } 
    // Радиус области в км    
    public required double RadiusKm { get; init; }
    // Целевая длина маршрута в км     
    public required double TargetLengthKm { get; init; }
    // Допустимое отклонение (10%)   
    public double LengthTolerance { get; init; } = 0.1;
    // Профиль маршрута
    public RouteProfile Profile { get; init; } = RouteProfile.Car;
}

public enum RouteProfile
{
    Car,
    Bike,
    Foot
}
