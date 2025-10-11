using CarPark.Data;
using CarPark.Shared.DateTimes;
using CarPark.TrackGenerator.Interfaces;
using CarPark.TrackGenerator.Models;
using CarPark.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarPark.TrackGenerator.Services
{
    public class TrackForceTimeWriterService
    {
        private readonly ITrackGenerationService _trackGenerationService;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<TrackRealTimeWriterService> _logger;

        public TrackForceTimeWriterService(
            ITrackGenerationService trackGenerationService,
            ApplicationDbContext dbContext,
            ILogger<TrackRealTimeWriterService> logger)
        {
            _trackGenerationService = trackGenerationService;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task GenerateAndWriteTrackAsync(Guid vehicleId, TrackGenerationOptions options, int updateIntervalSeconds)
        {
            _logger.LogInformation("Начинаем генерацию трека для автомобиля {VehicleId}", vehicleId);

            // 1. Генерируем трек
            _logger.LogInformation("Генерируем маршрут и точки трека...");
            Track track = await _trackGenerationService.GenerateTrackAsync(options);

            _logger.LogInformation("Трек сгенерирован: {PointsCount} точек", track.Points.Count);
            _logger.LogInformation("Продолжительность: {Duration}", track.Points.Last().Timestamp - track.Points.First().Timestamp);

            // 2. Инициализируем переменные для отслеживания
            int processedPoints = 0;
            DateTimeOffset startTime = DateTimeOffset.UtcNow;
            DateTimeOffset endTime = track.Points.Last().Timestamp;

            _logger.LogInformation("Начало трека: {StartTime}", startTime);
            _logger.LogInformation("Окончание трека: {EndTime}", endTime);

            // 3. Основной цикл записи

            DateTimeOffset currentTime = DateTimeOffset.UtcNow;
            while (true)
            {
                List<GeoTimePoint> pointsToWrite = track.Points
                    .Skip(processedPoints)
                    .Where(point => point.Timestamp <= currentTime)
                    .ToList();

                if (pointsToWrite.Count > 0)
                {
                    // Записываем точки в БД
                    await WritePointsToDatabase(vehicleId, pointsToWrite);
                    processedPoints += pointsToWrite.Count;

                    // Логируем прогресс
                    double progressPercent = (double)processedPoints / track.Points.Count * 100;
                    GeoTimePoint lastPoint = pointsToWrite.Last();

                    _logger.LogInformation("Записано точек: {ProcessedPoints}/{TotalPoints} ({ProgressPercent:F1}%)",
                        processedPoints, track.Points.Count, progressPercent);
                    _logger.LogInformation("Последняя позиция: {Latitude:F6}, {Longitude:F6}",
                        lastPoint.Location.Y, lastPoint.Location.X);
                    _logger.LogInformation("Скорость: {Speed:F1} км/ч", lastPoint.SpeedKmH);
                }

                // Проверяем завершение
                if (processedPoints >= track.Points.Count)
                {
                    _logger.LogInformation("Все точки трека записаны!");
                    _logger.LogInformation("Итого обработано: {ProcessedPoints} точек", processedPoints);
                    _logger.LogInformation("Время работы: {WorkingTime}", DateTimeOffset.UtcNow - startTime);
                    break;
                }

                currentTime += TimeSpan.FromSeconds(updateIntervalSeconds);
            }

            _logger.LogInformation("Запись трека для автомобиля {VehicleId} завершена", vehicleId);
        }

        private async Task WritePointsToDatabase(Guid vehicleId, List<GeoTimePoint> points)
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
}