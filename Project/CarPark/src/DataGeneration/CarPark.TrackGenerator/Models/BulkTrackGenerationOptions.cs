using NetTopologySuite.Geometries;

namespace CarPark.TrackGenerator.Models;

public class BulkTrackGenerationOptions
{
    /// <summary>
    /// Время начала периода генерации
    /// </summary>
    public required DateTimeOffset StartDate { get; init; }
    /// <summary>
    /// Время окончания периода генерации
    /// </summary>
    public required DateTimeOffset EndDate { get; init; }
    /// <summary>
    /// Доля дней, когда автомобиль активен (0.0 - 1.0)
    /// </summary>
    public required double ActiveDaysRatio { get; init; }
    /// <summary>
    /// Минимальная средняя дневная дистанция в км
    /// </summary>
    public required double MinAvgDailyDistanceKm { get; init; }
    /// <summary>
    /// Максимальная средняя дневная дистанция в км
    /// </summary>
    public required double MaxAvgDailyDistanceKm { get; init; }
    /// <summary>
    /// Размер батча для обработки
    /// </summary>
    public required int BatchSize { get; init; }
    /// <summary>
    /// Центральная точка области
    /// </summary>
    public required Point CenterPoint { get; init; }
    /// <summary>
    /// Радиус области в км
    /// </summary>
    public required double RadiusKm { get; init; }
    /// <summary>
    /// Максимальная скорость км/ч
    /// </summary>
    public required double MaxSpeedKmH { get; init; }
    /// <summary>
    /// Минимальная скорость км/ч
    /// </summary>
    public required double MinSpeedKmH { get; init; }
    /// <summary>
    /// Максимальное ускорение км/ч²
    /// </summary>
    public required double MaxAccelerationKmH2 { get; init; }
    /// <summary>
    /// Интервал между точками
    /// </summary>
    public required TimeSpan PointInterval { get; init; }
    /// <summary>
    /// Разброс интервала ±сек
    /// </summary>
    public required TimeSpan IntervalVariation { get; init; }
    /// <summary>
    /// Строка подключения к БД
    /// </summary>
    public required string ConnectionString { get; init; }
    /// <summary>
    /// API ключ GraphHopper
    /// </summary>
    public required string GraphHopperApiKey { get; init; }
}