using NetTopologySuite.Geometries;

namespace CarPark.TrackGenerator.Models;

public class TrackGenerationOptions
{
    /// <summary>
    /// Центр области
    /// </summary>
    public required Point CenterPoint { get; init; }
    /// <summary>
    /// Радиус области в км
    /// </summary>
    public required double RadiusKm { get; init; }
    /// <summary>
    /// Целевая длина маршрута в км
    /// </summary>
    public required double TargetLengthKm { get; init; }

    /// <summary>
    /// Максимальная скорость км/ч
    /// </summary>
    public required double MaxSpeedKmH { get; init; }
    /// <summary>
    /// Максимальное ускорение км/ч²
    /// </summary>
    public required double MaxAccelerationKmH2 { get; init; }
    /// <summary>
    /// Минимальная скорость км/ч
    /// </summary>
    public required double MinSpeedKmH { get; init; }

    /// <summary>
    /// Интервал точек
    /// </summary>
    public required TimeSpan PointInterval { get; init; }
    /// <summary>
    /// Разброс ±сек
    /// </summary>
    public required TimeSpan IntervalVariation { get; init; }
    /// <summary>
    /// Время старта трека
    /// </summary>
    public required DateTimeOffset StartTime { get; init; }
}
