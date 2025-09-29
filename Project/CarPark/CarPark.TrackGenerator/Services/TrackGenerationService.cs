using CarPark.TrackGenerator.Interfaces;
using CarPark.TrackGenerator.Models;
using NetTopologySuite.Geometries;
using NetTopologySuite.LinearReferencing;

namespace CarPark.TrackGenerator.Services;

public class TrackGenerationService : ITrackGenerationService
{
    private readonly IRouteGenerationService _routeGenerationService;
    private readonly Random _random = new();

    public TrackGenerationService(IRouteGenerationService routeGenerationService)
    {
        _routeGenerationService = routeGenerationService;
    }

    public async Task<Track> GenerateTrackAsync(TrackGenerationOptions options)
    {
        // 1. Получаем маршрут через RouteGenerationService
        var routeOptions = new RouteGenerationOptions
        {
            CenterPoint = options.CenterPoint,
            RadiusKm = options.RadiusKm,
            TargetLengthKm = options.TargetLengthKm,
            Profile = RouteProfile.Car
        };

        LineString route = await _routeGenerationService.GenerateRouteAsync(routeOptions);

        // 2. Генерируем точки трека с временными метками
        var points = GenerateGeoTimePoints(route, options);

        // 3. Создаем трек
        return new Track
        {
            VehicleId = Guid.NewGuid(), // Будет переопределен вызывающим кодом
            Points = points
        };
    }

    private List<GeoTimePoint> GenerateGeoTimePoints(LineString route, TrackGenerationOptions options)
    {
        var points = new List<GeoTimePoint>();
        var lengthIndexedLine = new LengthIndexedLine(route);
        
        double totalLength = route.Length; // длина в единицах координатной системы
        double currentDistance = 0;
        DateTimeOffset currentTime = options.StartTime;
        double currentSpeedKmH = options.MinSpeedKmH; // Начинаем с минимальной скорости
        double currentAcceleration = 0;

        // 1. Добавляем начальную точку маршрута
        Coordinate startCoordinate = lengthIndexedLine.ExtractPoint(0);
        Point startLocation = new Point(new Coordinate(startCoordinate.X, startCoordinate.Y)) { SRID = 4326 };
        points.Add(new GeoTimePoint
        {
            Location = startLocation,
            Timestamp = currentTime,
            SpeedKmH = currentSpeedKmH,
            AccelerationKmH2 = currentAcceleration
        });

        // 2. Генерируем промежуточные точки
        while (true)
        {
            // Генерируем параметры для следующей точки
            TimeSpan nextInterval = GenerateNextInterval(options);
            (double nextSpeed, double acceleration) = GenerateNextSpeedAndAcceleration(currentSpeedKmH, nextInterval, options);

            // Рассчитываем расстояние до следующей точки
            double distanceIncrement = currentSpeedKmH * nextInterval.TotalHours; // км
            double distanceInRouteUnits = ConvertKilometersToWgs84DegreesSimple(distanceIncrement);

            // Проверяем, не выходим ли за пределы маршрута
            double nextDistance = currentDistance + distanceInRouteUnits;
            
            if (nextDistance >= totalLength)
            {
                // Если следующая точка выходит за пределы маршрута, прекращаем генерацию
                break;
            }

            // Переходим к следующей точке
            currentDistance = nextDistance;
            currentTime = currentTime.Add(nextInterval);
            currentSpeedKmH = nextSpeed;
            currentAcceleration = acceleration;

            // Получаем координату точки на текущем расстоянии
            Coordinate coordinate = lengthIndexedLine.ExtractPoint(currentDistance);
            Point location = new Point(new Coordinate(coordinate.X, coordinate.Y)) { SRID = 4326 };

            // Создаем точку трека
            points.Add(new GeoTimePoint
            {
                Location = location,
                Timestamp = currentTime,
                SpeedKmH = currentSpeedKmH,
                AccelerationKmH2 = currentAcceleration
            });
        }

        // 3. Добавляем конечную точку маршрута
        if (points.Count > 1) // Если у нас есть промежуточные точки
        {
            var lastPoint = points[^1];
            var timeToEnd = CalculateTimeToReachEnd(currentDistance, totalLength, currentSpeedKmH);
            var endTime = lastPoint.Timestamp.Add(timeToEnd);
            
            Coordinate endCoordinate = lengthIndexedLine.ExtractPoint(totalLength);
            Point endLocation = new Point(new Coordinate(endCoordinate.X, endCoordinate.Y)) { SRID = 4326 };

            points.Add(new GeoTimePoint
            {
                Location = endLocation,
                Timestamp = endTime,
                SpeedKmH = currentSpeedKmH,
                AccelerationKmH2 = currentAcceleration
            });
        }

        return points;
    }

    /// <summary>
    /// Рассчитывает время, необходимое для достижения конца маршрута
    /// </summary>
    private TimeSpan CalculateTimeToReachEnd(double currentDistance, double totalLength, double currentSpeedKmH)
    {
        double remainingDistanceInRouteUnits = totalLength - currentDistance;
        double remainingDistanceKm = ConvertWgs84DegreesToKilometersSimple(remainingDistanceInRouteUnits);
        
        // Время = Расстояние / Скорость
        double remainingTimeHours = remainingDistanceKm / currentSpeedKmH;
        
        return TimeSpan.FromHours(remainingTimeHours);
    }

    /// <summary>
    /// Обратная конвертация: из градусов WGS84 в километры (для коротких расстояний)
    /// </summary>
    private double ConvertWgs84DegreesToKilometersSimple(double degrees)
    {
        const double KmPerDegree = 111.0;
        return degrees * KmPerDegree;
    }

    private TimeSpan GenerateNextInterval(TrackGenerationOptions options)
    {
        double variationSeconds = options.IntervalVariation.TotalSeconds;
        double randomVariation = (_random.NextDouble() - 0.5) * 2 * variationSeconds; // -variation...+variation
        
        var interval = options.PointInterval.Add(TimeSpan.FromSeconds(randomVariation));
        
        // Минимальный интервал 1 секунда
        return interval.TotalSeconds < 1 ? TimeSpan.FromSeconds(1) : interval;
    }

    private (double newSpeed, double acceleration) GenerateNextSpeedAndAcceleration(double currentSpeed, TimeSpan timeInterval, TrackGenerationOptions options)
    {
        // Генерируем случайное ускорение от -1 до +1, умножаем на максимальное
        double acceleration = (_random.NextDouble() * 2 - 1) * options.MaxAccelerationKmH2; // -1 to +1
        
        // Вычисляем новую скорость: v = v₀ + a·t
        double timeHours = timeInterval.TotalHours;
        double newSpeed = currentSpeed + acceleration * timeHours;
        
        // Ограничиваем скорость пределами
        newSpeed = Math.Max(options.MinSpeedKmH, Math.Min(options.MaxSpeedKmH, newSpeed));
        
        // Корректируем ускорение если скорость была ограничена
        double actualAcceleration = timeHours > 0 ? (newSpeed - currentSpeed) / timeHours : 0;
        
        return (newSpeed, actualAcceleration);
    }

    /// <summary>
    /// Простое вычисление расстояния в градусах WGS84 для коротких расстояний
    /// Использует фиксированный коэффициент для городских маршрутов
    /// </summary>
    private double ConvertKilometersToWgs84DegreesSimple(double distanceKm)
    {
        // Для коротких расстояний (до ~50км) используем упрощенный коэффициент
        // 1 градус ≈ 111.32 км на экваторе
        // Для средних широт (45-60°) берем среднее значение ~111 км
        const double KmPerDegree = 111.0;
        
        return distanceKm / KmPerDegree;
    }
}
