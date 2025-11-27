using CarPark.Data;
using CarPark.DateTimes;
using CarPark.TrackGenerator.GraphHopper;
using CarPark.TrackGenerator.Interfaces;
using CarPark.TrackGenerator.Models;
using CarPark.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace CarPark.TrackGenerator.Services;

public class BulkTrackGenerationService
{
    private readonly ITrackGenerationService _trackGenerationService;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<BulkTrackGenerationService> _logger;

    public BulkTrackGenerationService(
        ITrackGenerationService trackGenerationService,
        ApplicationDbContext dbContext,
        ILogger<BulkTrackGenerationService> logger)
    {
        _trackGenerationService = trackGenerationService;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task GenerateTracksForAllVehiclesAsync(List<Guid> vehicleIds, BulkTrackGenerationOptions options)
    {
        _logger.LogInformation("Начинаем массовую генерацию треков для {VehicleCount} автомобилей", vehicleIds.Count);

        int totalVehicles = vehicleIds.Count;
        int processedVehicles = 0;

        // Обрабатываем автомобили батчами
        for (int i = 0; i < totalVehicles; i += options.BatchSize)
        {
            List<Guid> batch = vehicleIds.Skip(i).Take(options.BatchSize).ToList();
            _logger.LogInformation("Обработка батча {BatchNumber}: {BatchSize} автомобилей", i / options.BatchSize + 1, batch.Count);

            await ProcessBatchAsync(batch, options);

            processedVehicles += batch.Count;
            double progressPercent = (double)processedVehicles / totalVehicles * 100;
            _logger.LogInformation("Прогресс: {ProcessedVehicles}/{TotalVehicles} автомобилей ({ProgressPercent:F1}%)",
                processedVehicles, totalVehicles, progressPercent);
        }

        _logger.LogInformation("Массовая генерация треков завершена для {TotalVehicles} автомобилей", totalVehicles);
    }

    private async Task ProcessBatchAsync(List<Guid> vehicleIds, BulkTrackGenerationOptions options)
    {
        // Загружаем автомобили из БД
        List<Vehicle> vehicles = await _dbContext.Vehicles
            .Where(v => vehicleIds.Contains(v.Id))
            .ToListAsync();

        if (vehicles.Count != vehicleIds.Count)
        {
            List<Guid> missingIds = vehicleIds.Except(vehicles.Select(v => v.Id)).ToList();
            _logger.LogWarning("Некоторые автомобили не найдены в БД: {MissingIds}", string.Join(", ", missingIds));
        }

        // Генерируем треки для каждого автомобиля параллельно
        List<Task> generationTasks = vehicles.Select(vehicle =>
            GenerateTracksForVehicleAsync(vehicle, options)).ToList();

        await Task.WhenAll(generationTasks);
    }

    private async Task GenerateTracksForVehicleAsync(Vehicle vehicle, BulkTrackGenerationOptions options)
    {
        _logger.LogInformation("Генерация треков для автомобиля {VehicleId}", vehicle.Id);

        // Определяем активные дни для автомобиля
        List<DateTimeOffset> activeDays = GenerateActiveDays(options.StartDate, options.EndDate, options.ActiveDaysRatio);

        _logger.LogInformation("Автомобиль {VehicleId} активен в {ActiveDaysCount} дней из {TotalDays}",
            vehicle.Id, activeDays.Count, (options.EndDate - options.StartDate).TotalDays);

        // Собираем все точки трека для автомобиля
        List<GeoTimePoint> allTrackPoints = new List<GeoTimePoint>();

        // Точка окончания предыдущего дня (для непрерывности)
        Point? lastEndPoint = null;

        // Генерируем треки для каждого активного дня
        foreach (DateTimeOffset activeDay in activeDays)
        {
            List<GeoTimePoint> dayPoints = await GenerateTracksForDayAsync(vehicle, activeDay, options, lastEndPoint);
            allTrackPoints.AddRange(dayPoints);

            // Обновляем последнюю точку для следующего дня
            if (dayPoints.Any())
            {
                lastEndPoint = new Point(new Coordinate(dayPoints.Last().Location.X, dayPoints.Last().Location.Y)) { SRID = 4326 };
            }
        }

        // Записываем все точки трека автомобиля в БД сразу
        if (allTrackPoints.Any())
        {
            await WriteTrackPointsToDatabaseAsync(vehicle.Id, allTrackPoints);
            _logger.LogInformation("Записано {TotalPoints} точек для автомобиля {VehicleId}",
                allTrackPoints.Count, vehicle.Id);
        }

        _logger.LogInformation("Генерация треков для автомобиля {VehicleId} завершена", vehicle.Id);
    }

    private async Task<List<GeoTimePoint>> GenerateTracksForDayAsync(Vehicle vehicle, DateTimeOffset day, BulkTrackGenerationOptions options, Point? lastEndPoint = null)
    {
        // Определяем, активен ли автомобиль в этот день
        bool isActive = Random.Shared.NextDouble() < options.ActiveDaysRatio;
        if (!isActive)
        {
            _logger.LogDebug("Автомобиль {VehicleId} не активен {Date:yyyy-MM-dd}", vehicle.Id, day.Date);
            return new List<GeoTimePoint>();
        }

        // Генерируем общую дистанцию для дня в заданном диапазоне
        double dailyDistance = Random.Shared.NextDouble() * (options.MaxAvgDailyDistanceKm - options.MinAvgDailyDistanceKm) + options.MinAvgDailyDistanceKm;

        _logger.LogInformation("Генерация треков для автомобиля {VehicleId} на {Date:yyyy-MM-dd}: {DailyDistance:F1} км",
            vehicle.Id, day.Date, dailyDistance);

        // Генерируем непрерывный трек для дня
        return await GenerateContinuousTrackAsync(vehicle, day, dailyDistance, options, lastEndPoint);
    }

    private async Task<List<GeoTimePoint>> GenerateContinuousTrackAsync(Vehicle vehicle, DateTimeOffset day, double dailyDistance, BulkTrackGenerationOptions options, Point? lastEndPoint = null)
    {
        // Определяем точку старта: либо последняя точка предыдущего дня, либо случайная в радиусе
        Point startPoint;
        if (lastEndPoint != null)
        {
            // Начинаем с последней точки предыдущего дня
            startPoint = lastEndPoint;
            _logger.LogDebug("Автомобиль {VehicleId} продолжает с точки предыдущего дня: {Lat:F6}, {Lon:F6}",
                vehicle.Id, startPoint.Y, startPoint.X);
        }
        else
        {
            // Первая поездка - генерируем случайную точку старта
            startPoint = GenerateRandomPointInRadius(options.CenterPoint, options.RadiusKm);
            _logger.LogDebug("Автомобиль {VehicleId} начинает с новой точки: {Lat:F6}, {Lon:F6}",
                vehicle.Id, startPoint.Y, startPoint.X);
        }

        // Генерируем случайное время начала движения в течение дня
        TimeSpan randomTime = TimeSpan.FromHours(Random.Shared.Next(6, 22)); // с 6:00 до 22:00
        DateTimeOffset trackStartTime = day.Date.Add(randomTime);

        // Создаем опции для генерации непрерывного трека
        TrackGenerationOptions trackOptions = new TrackGenerationOptions
        {
            CenterPoint = startPoint,
            RadiusKm = options.RadiusKm,
            TargetLengthKm = dailyDistance,
            MaxSpeedKmH = options.MaxSpeedKmH,
            MinSpeedKmH = options.MinSpeedKmH,
            MaxAccelerationKmH2 = options.MaxAccelerationKmH2,
            PointInterval = options.PointInterval,
            IntervalVariation = options.IntervalVariation,
            StartTime = trackStartTime
        };

        try
        {
            // Генерируем непрерывный трек для дня
            Track track = await _trackGenerationService.GenerateTrackAsync(trackOptions);

            _logger.LogDebug("Непрерывный трек для автомобиля {VehicleId} сгенерирован: {PointsCount} точек, длина {Length:F1} км",
                vehicle.Id, track.Points.Count, dailyDistance);

            return track.Points.ToList();
        }
        catch (GraphHopperApiException ex) when (ex.Message.Contains("Minutely API limit heavily violated"))
        {
            _logger.LogWarning("Генерация трека для автомобиля {VehicleId} пропущена из-за превышения лимита API GraphHopper: {Message}", vehicle.Id, ex.Message);
            return new List<GeoTimePoint>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка генерации непрерывного трека для автомобиля {VehicleId}", vehicle.Id);
            return new List<GeoTimePoint>();
        }
    }

    private Point GenerateRandomPointInRadius(Point center, double radiusKm)
    {
        // Генерируем случайную точку в круге
        double angle = Random.Shared.NextDouble() * 2 * Math.PI;
        double radius = Math.Sqrt(Random.Shared.NextDouble()) * radiusKm; // Равномерное распределение в круге

        // Конвертируем в смещение по координатам (приближенно)
        double deltaLat = (radius * Math.Cos(angle)) / 111.0; // ~111 км на градус широты
        double deltaLon = (radius * Math.Sin(angle)) / (111.0 * Math.Cos(center.Y * Math.PI / 180.0));

        return new Point(new Coordinate(center.X + deltaLon, center.Y + deltaLat)) { SRID = 4326 };
    }

    private List<DateTimeOffset> GenerateActiveDays(DateTimeOffset startDate, DateTimeOffset endDate, double activeRatio)
    {
        List<DateTimeOffset> allDays = new List<DateTimeOffset>();
        for (DateTimeOffset date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            allDays.Add(date);
        }

        // Выбираем случайные дни на основе коэффициента активности
        int activeDaysCount = (int)Math.Round(allDays.Count * activeRatio);
        return allDays.OrderBy(_ => Random.Shared.Next()).Take(activeDaysCount).OrderBy(d => d).ToList();
    }


    private async Task WriteTrackPointsToDatabaseAsync(Guid vehicleId, List<GeoTimePoint> points)
    {
        // Загружаем Vehicle из БД для связи
        Vehicle? vehicle = await _dbContext.Vehicles.FirstOrDefaultAsync(v => v.Id == vehicleId);
        if (vehicle == null)
        {
            throw new InvalidOperationException($"Автомобиль с ID {vehicleId} не найден в базе данных");
        }

        List<VehicleGeoTimePoint> dbPoints = points.Select(point => VehicleGeoTimePoint.Create(
            Guid.NewGuid(),
            vehicle,
            point.Location,
            new UtcDateTimeOffset(point.Timestamp.UtcDateTime)
        ).Value).ToList();

        _dbContext.VehicleGeoTimePoints.AddRange(dbPoints);
        await _dbContext.SaveChangesAsync();

        _logger.LogDebug("Записано {Count} точек в БД для автомобиля {VehicleId}", points.Count, vehicleId);
    }
}