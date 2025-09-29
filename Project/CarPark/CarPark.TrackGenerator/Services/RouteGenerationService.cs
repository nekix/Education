using CarPark.TrackGenerator.GraphHopper;
using CarPark.TrackGenerator.GraphHopper.Models;
using CarPark.TrackGenerator.Interfaces;
using NetTopologySuite.Geometries;
using Microsoft.Extensions.Logging;

namespace CarPark.TrackGenerator.Services;

public class RouteGenerationService : IRouteGenerationService
{
    private readonly IGraphHopperApiClient _graphHopperClient;
    private readonly ILogger<RouteGenerationService> _logger;
    private readonly Random _random = new();
    private readonly GeometryFactory _geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);

    public RouteGenerationService(
        IGraphHopperApiClient graphHopperClient,
        ILogger<RouteGenerationService> logger)
    {
        _graphHopperClient = graphHopperClient;
        _logger = logger;
    }

    public async Task<LineString> GenerateRouteAsync(RouteGenerationOptions options)
    {
        // 1) Генерируем случайную начальную точку
        Point startPoint = GenerateRandomPointInRadius(options.CenterPoint, options.RadiusKm);
        List<Point> routePoints = new List<Point> { startPoint };

        double directRouteLengthKm = 0;

        do
        {
            // 2) Генерируем следующую случайную точку
            Point nextPoint = GenerateNextRandomPoint(routePoints[^1], options.CenterPoint, options.RadiusKm);

            // 3) Добавляем точку к маршруту
            routePoints.Add(nextPoint);

            // 4) Смотрим длину маршрута "напрямую"
            directRouteLengthKm = CalculateDistanceKm(routePoints);
        }
        while (directRouteLengthKm < options.TargetLengthKm - options.LengthTolerance);

        // Если длина маршрута напрямую оказалась достаточной,
        // то реальная длина маршрута точно будет досаточной.

        // 4) Строим маршрут для заданных точек
        Coordinate[] routeCoordinates = await GenerateMultiPointRouteAsync(routePoints, options.Profile);

        double realRouteLengthKm = CalculateSegmentLengthKm(routeCoordinates);

        if (Math.Abs(realRouteLengthKm - options.TargetLengthKm) <= options.TargetLengthKm * options.LengthTolerance)
        {
            // Длина равна заданной - возвращаем маршрут
            return _geometryFactory.CreateLineString(routeCoordinates);
        }
        else
        {
            // Длина больше заданной - обрезаем с конца до заданной длины
            Coordinate[] trimmedRoute = TrimRouteToLength(routeCoordinates, options.TargetLengthKm);
            return _geometryFactory.CreateLineString(trimmedRoute);
        }
    }

    private Point GenerateNextRandomPoint(Point startPoint, Point centerPoint, double radiusKm)
    {
        double minDistanceKm = radiusKm / 2.0; // Минимум 1/2 радиуса от начальной точки

        Point nextPoint;

        do
        {
            nextPoint = GenerateRandomPointInRadius(centerPoint, radiusKm);
        }
        while (CalculateDistanceKm(nextPoint, startPoint) < minDistanceKm);

        return nextPoint;
    }

    private async Task<Coordinate[]> GenerateMultiPointRouteAsync(List<Point> points, RouteProfile profile)
    {
        double[][] pointsArray = points.Select(p => new double[] { p.X, p.Y }).ToArray();
        
        RouteRequest request = new RouteRequest
        {
            Points = pointsArray,
            Profile = ConvertProfileToString(profile),
            PointsEncoded = false,
            CalcPoints = true,
            Instructions = false,
            Elevation = false
        };

        RouteResponse response = await _graphHopperClient.GetRouteAsync(request);
        
        if (response.Paths.Length == 0 || response.Paths[0].Points?.Coordinates == null)
        {
            throw new InvalidOperationException("GraphHopper API returned empty route");
        }

        return response.Paths[0].Points!.Coordinates
            .Select(coord => new Coordinate(coord[0], coord[1]))
            .ToArray();
    }

    private Coordinate[] TrimRouteToLength(Coordinate[] route, double targetLengthKm)
    {
        List<Coordinate> trimmedRoute = new List<Coordinate>();
        double accumulatedLength = 0;
        
        for (int i = 0; i < route.Length - 1; i++)
        {
            trimmedRoute.Add(route[i]);
            
            Point point1 = new Point(new Coordinate(route[i].X, route[i].Y)) { SRID = 4326 };
            Point point2 = new Point(new Coordinate(route[i + 1].X, route[i + 1].Y)) { SRID = 4326 };
            double segmentLength = CalculateDistanceKm(point1, point2);
            
            if (accumulatedLength + segmentLength >= targetLengthKm)
            {
                // Находим точку на нужном расстоянии внутри сегмента
                double remainingLength = targetLengthKm - accumulatedLength;
                double ratio = remainingLength / segmentLength;
                
                double finalX = route[i].X + (route[i + 1].X - route[i].X) * ratio;
                double finalY = route[i].Y + (route[i + 1].Y - route[i].Y) * ratio;
                
                trimmedRoute.Add(new Coordinate(finalX, finalY));
                break;
            }
            
            accumulatedLength += segmentLength;
        }
        
        // Если не достигли целевой длины, добавляем последнюю точку
        if (trimmedRoute.Count < 2)
        {
            trimmedRoute.Add(route[^1]);
        }
        
        return trimmedRoute.ToArray();
    }

    private double CalculateSegmentLengthKm(Coordinate[] segment)
    {
        double totalLength = 0;
        
        for (int i = 0; i < segment.Length - 1; i++)
        {
            Point point1 = new Point(new Coordinate(segment[i].X, segment[i].Y)) { SRID = 4326 };
            Point point2 = new Point(new Coordinate(segment[i + 1].X, segment[i + 1].Y)) { SRID = 4326 };
            totalLength += CalculateDistanceKm(point1, point2);
        }
        
        return totalLength;
    }

    private Point GenerateRandomPointInRadius(Point center, double radiusKm)
    {
        // Генерируем случайную точку в круге
        double angle = _random.NextDouble() * 2 * Math.PI;
        double radius = Math.Sqrt(_random.NextDouble()) * radiusKm; // Равномерное распределение в круге
        
        // Конвертируем в смещение по координатам (приближенно)
        double deltaLat = (radius * Math.Cos(angle)) / 111.0; // ~111 км на градус широты
        double deltaLon = (radius * Math.Sin(angle)) / (111.0 * Math.Cos(center.Y * Math.PI / 180.0));
        
        return new Point(new Coordinate(center.X + deltaLon, center.Y + deltaLat)) { SRID = 4326 };
    }

    private double CalculateDistanceKm(List<Point> points)
    {
        if (points.Count < 2)
            return 0;

        double distanceKm = 0;

        for (int i = 0; i < points.Count - 1; i++)
        {
            distanceKm += CalculateDistanceKm(points[i], points[i + 1]);
        }

        return distanceKm;
    }

    private double CalculateDistanceKm(Point point1, Point point2)
    {
        // Простое приближение для коротких расстояний
        double deltaLat = point2.Y - point1.Y;
        double deltaLon = point2.X - point1.X;
        double avgLat = (point1.Y + point2.Y) / 2 * Math.PI / 180.0;
        
        double distanceLat = deltaLat * 111.0;
        double distanceLon = deltaLon * 111.0 * Math.Cos(avgLat);
        
        return Math.Sqrt(distanceLat * distanceLat + distanceLon * distanceLon);
    }


    private string ConvertProfileToString(RouteProfile profile)
    {
        return profile switch
        {
            RouteProfile.Car => "car",
            RouteProfile.Bike => "bike",
            RouteProfile.Foot => "foot",
            _ => "car"
        };
    }
}


