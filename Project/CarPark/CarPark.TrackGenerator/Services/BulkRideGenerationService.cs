using CarPark.Data;
using CarPark.Rides;
using CarPark.Shared.DateTimes;
using CarPark.TrackGenerator.Models;
using CarPark.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace CarPark.TrackGenerator.Services;

public class BulkRideGenerationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Random _random = new();

    public BulkRideGenerationService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task GenerateRidesForAllVehiclesAsync(
        List<Guid> vehicleIds,
        BulkRideGenerationOptions options)
    {
        Console.WriteLine("Bulk Ride Generation");
        Console.WriteLine($"Period: {options.StartDate:yyyy-MM-dd} to {options.EndDate:yyyy-MM-dd}");
        Console.WriteLine($"Active days ratio: {options.ActiveDaysRatio:P0}");
        Console.WriteLine($"Average rides per day: {options.AverageRidesPerDay}");
        Console.WriteLine($"Ride duration: {options.MinRideDurationMinutes}-{options.MaxRideDurationMinutes} min");
        Console.WriteLine();

        Console.WriteLine($"Processing {vehicleIds.Count} vehicles");

        int totalRidesGenerated = 0;

        foreach (var vehicleId in vehicleIds)
        {
            var ridesCount = await GenerateRidesForVehicleAsync(vehicleId, options);
            totalRidesGenerated += ridesCount;
            Console.WriteLine($"  Vehicle {vehicleId}: {ridesCount} rides generated");
        }

        Console.WriteLine($"Total rides generated: {totalRidesGenerated}");
    }

    private async Task<int> GenerateRidesForVehicleAsync(
        Guid vehicleId,
        BulkRideGenerationOptions options)
    {
        // Получаем все точки трека для автомобиля в заданном периоде
        List<VehicleGeoTimePoint> trackPoints = await _dbContext.VehicleGeoTimePoints
            .Include(p => p.Vehicle)
            .Where(p => p.Vehicle.Id == vehicleId &&
                       p.Time >= new UtcDateTimeOffset(options.StartDate) &&
                       p.Time <= new UtcDateTimeOffset(options.EndDate))
            .OrderBy(p => p.Time)
            .ToListAsync();

        if (!trackPoints.Any())
        {
            return 0;
        }

        // Определяем активные дни
        var activeDays = GetActiveDays(options.StartDate, options.EndDate, options.ActiveDaysRatio);

        int ridesGenerated = 0;

        foreach (var activeDay in activeDays)
        {
            var dayStart = new UtcDateTimeOffset(activeDay.Date);
            var dayEnd = new UtcDateTimeOffset(activeDay.Date.AddDays(1));

            // Получаем точки трека за этот день
            var dayPoints = trackPoints
                .Where(p => p.Time >= dayStart && p.Time < dayEnd)
                .OrderBy(p => p.Time)
                .ToList();

            if (!dayPoints.Any())
                continue;

            // Генерируем поездки для этого дня
            var dayRidesCount = GenerateRidesForDay(vehicleId, dayPoints, options);
            ridesGenerated += dayRidesCount;
        }

        return ridesGenerated;
    }

    private List<DateTime> GetActiveDays(DateTimeOffset startDate, DateTimeOffset endDate, double activeDaysRatio)
    {
        var activeDays = new List<DateTime>();
        var currentDate = startDate.Date;

        while (currentDate <= endDate.Date)
        {
            if (_random.NextDouble() < activeDaysRatio)
            {
                activeDays.Add(currentDate);
            }
            currentDate = currentDate.AddDays(1);
        }

        return activeDays;
    }

    private int GenerateRidesForDay(
        Guid vehicleId,
        List<VehicleGeoTimePoint> dayPoints,
        BulkRideGenerationOptions options)
    {
        if (dayPoints.Count < 2)
            return 0;

        // Определяем количество поездок для дня (нормальное распределение вокруг среднего)
        int ridesCount = GenerateNormalDistributedCount(options.AverageRidesPerDay);
        ridesCount = Math.Max(1, Math.Min(ridesCount, dayPoints.Count / 2)); // Минимум 1, максимум половина точек

        var rides = new List<Ride>();
        var usedTimeRanges = new List<(DateTimeOffset Start, DateTimeOffset End)>();

        for (int i = 0; i < ridesCount; i++)
        {
            var ride = TryGenerateRide(vehicleId, dayPoints, options, usedTimeRanges);
            if (ride != null)
            {
                rides.Add(ride);
                usedTimeRanges.Add((ride.StartTime, ride.EndTime));
            }
        }

        if (rides.Any())
        {
            _dbContext.Rides.AddRange(rides);
            _dbContext.SaveChanges();
        }

        return rides.Count;
    }

    private Ride? TryGenerateRide(
        Guid vehicleId,
        List<VehicleGeoTimePoint> dayPoints,
        BulkRideGenerationOptions options,
        List<(DateTimeOffset Start, DateTimeOffset End)> usedTimeRanges)
    {
        const int maxAttempts = 10;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Выбираем случайную начальную точку
            var startIndex = _random.Next(0, dayPoints.Count - 1);
            var startPoint = dayPoints[startIndex];

            // Генерируем продолжительность поездки
            var durationMinutes = _random.Next(options.MinRideDurationMinutes, options.MaxRideDurationMinutes + 1);
            var duration = TimeSpan.FromMinutes(durationMinutes);

            // Рассчитываем время окончания
            var rideStartTime = startPoint.Time.Value;
            var rideEndTime = rideStartTime.Add(duration);

            // Проверяем, что поездка не выходит за пределы дня
            if (rideEndTime > dayPoints.Last().Time.Value)
                continue;

            // Находим ближайшую точку к rideEndTime для endPoint
            var endPoint = dayPoints
                .Where(p => p.Time.Value >= rideStartTime && p.Time.Value <= rideEndTime)
                .OrderBy(p => Math.Abs((p.Time.Value - rideEndTime).TotalSeconds))
                .FirstOrDefault();

            if (endPoint == null)
                continue;

            // Синхронизируем rideEndTime с временем endPoint
            rideEndTime = endPoint.Time.Value;

            // Проверяем на пересечения с существующими поездками
            if (usedTimeRanges.Any(range =>
                (rideStartTime >= range.Start && rideStartTime <= range.End) ||
                (rideEndTime >= range.Start && rideEndTime <= range.End) ||
                (rideStartTime <= range.Start && rideEndTime >= range.End)))
            {
                continue; // Пересечение с другой поездкой
            }

            // Создаем поездку
            var ride = new Ride
            {
                Id = Guid.NewGuid(),
                Vehicle = startPoint.Vehicle,
                StartTime = rideStartTime,
                EndTime = rideEndTime,
                StartPoint = startPoint,
                EndPoint = endPoint
            };

            return ride;
        }

        return null; // Не удалось сгенерировать поездку после нескольких попыток
    }

    private int GenerateNormalDistributedCount(int average)
    {
        // Простая аппроксимация нормального распределения
        double u1 = 1.0 - _random.NextDouble();
        double u2 = 1.0 - _random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

        int result = (int)Math.Round(average + randStdNormal * (average * 0.3)); // Стандартное отклонение 30% от среднего
        return Math.Max(1, result);
    }
}