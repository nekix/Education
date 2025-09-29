using NetTopologySuite.Geometries;

namespace CarPark.TrackGenerator;

/// <summary>
/// Интерфейс для генерации маршрутов (будет реализован позже с GraphHopper API)
/// </summary>
public interface IRouteGenerator
{
    Task<LineString> GetNewRouteAsync(Point center, Point startPoint, int radiusKm, int routeLengthKm);
}
