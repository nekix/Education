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
    public double MaxSpeedKmH { get; init; } = 90;
    /// <summary>
    /// Максимальное ускорение км/ч²
    /// </summary>
    public double MaxAccelerationKmH2 { get; init; } = 20;
    /// <summary>
    /// Минимальная скорость км/ч
    /// </summary>
    public double MinSpeedKmH { get; init; } = 5;

    /// <summary>
    /// Интервал точек
    /// </summary>
    public TimeSpan PointInterval { get; init; } = TimeSpan.FromSeconds(10);
    /// <summary>
    /// Разброс ±сек
    /// </summary>
    public TimeSpan IntervalVariation { get; init; } = TimeSpan.FromSeconds(3);
    /// <summary>
    /// Время старта трека
    /// </summary>
    public DateTimeOffset StartTime { get; init; } = DateTimeOffset.Now;
}
