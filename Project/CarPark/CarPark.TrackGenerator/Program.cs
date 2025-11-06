using CarPark.Data;
using CarPark.Services.TimeZones;
using CarPark.TrackGenerator.GraphHopper;
using CarPark.TrackGenerator.Interfaces;
using CarPark.TrackGenerator.Models;
using CarPark.TrackGenerator.Services;
using CarPark.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using System.CommandLine;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CarPark.TrackGenerator;

public sealed class Program
{
    public static async Task<int> Main(string[] args)
    {
        RootCommand rootCommand = new RootCommand("Генератор треков автомобилей в реальном времени");

        rootCommand.Add(GenerateSingleTrackCommand());
        rootCommand.Add(GenerateBulkTracksCommand());
        rootCommand.Add(GenerateBulkRidesCommand());
        rootCommand.Add(GenerateTemplateCommand());
        rootCommand.Add(GenerateFromTemplatesCommand());

        return await rootCommand.Parse(args).InvokeAsync();
    }

    private static Command GenerateSingleTrackCommand()
    {
        // Параметры командной строки
        Option<Guid> vehicleIdOption = new Option<Guid>("--vehicle-id", "-v") { Description = "ID автомобиля для генерации трека", Required = true };
        Option<string> connectionStringOption = new Option<string>("--connection-string", "-c") { Description = "Строка подключения к БД", Required = true };
        Option<string> graphHopperApiKeyOption = new Option<string>("--api-key", "-k") { Description = "API ключ GraphHopper", Required = true };

        // Параметры трека
        Option<double> centerLatOption = new Option<double>("--center-lat", "-lat") { Description = "Широта центра области (например, 55.7558 для Москвы)", Required = true };
        Option<double> centerLonOption = new Option<double>("--center-lon", "-lon") { Description = "Долгота центра области (например, 37.6176 для Москвы)", Required = true };
        Option<bool> forceWriteOption = new Option<bool>("--force-write", "-for") { Description = "Сразу записать весь трек (в т.ч. точки, пройденные в будущем)", Required = true };

        Option<double> radiusKmOption = new Option<double>("--radius-km", "-r") { Description = "Радиус области в км", Required = true };
        Option<double> targetLengthKmOption = new Option<double>("--target-length-km", "-l") { Description = "Целевая длина трека в км", Required = true };
        Option<double> maxSpeedOption = new Option<double>("--max-speed", "-max-s") { Description = "Максимальная скорость км/ч", Required = true };
        Option<double> minSpeedOption = new Option<double>("--min-speed", "-min-s") { Description = "Минимальная скорость км/ч", Required = true };
        Option<double> maxAccelerationOption = new Option<double>("--max-acceleration", "-a") { Description = "Максимальное ускорение км/ч²", Required = true };

        // Шаг точек с разбросом
        Option<int> pointIntervalOption = new Option<int>("--point-interval", "-p") { Description = "Интервал между точками трека в секундах", Required = true };
        Option<int> intervalVariationOption = new Option<int>("--interval-variation", "-pv") { Description = "Разброс интервала между точками в секундах", Required = true };

        // Технические параметры (необязательные, но проще оставить обязательными)
        Option<int> updateIntervalSecondsOption = new Option<int>("--update-interval", "-i") { Description = "Интервал обновления в секундах", Required = true };

        // Команда генерации трека
        Command generateCommand = new Command("generate", "Генерация трека автомобиля")
        {
            vehicleIdOption,
            connectionStringOption,
            graphHopperApiKeyOption,
            centerLatOption,
            centerLonOption,
            radiusKmOption,
            targetLengthKmOption,
            maxSpeedOption,
            minSpeedOption,
            maxAccelerationOption,
            pointIntervalOption,
            intervalVariationOption,
            updateIntervalSecondsOption,
            forceWriteOption
        };

        generateCommand.SetAction(parseResult => GenerateTrackAsync(
            parseResult.GetValue(vehicleIdOption),
            parseResult.GetValue(connectionStringOption)!,
            parseResult.GetValue(graphHopperApiKeyOption)!,
            parseResult.GetValue(centerLatOption),
            parseResult.GetValue(centerLonOption),
            parseResult.GetValue(radiusKmOption),
            parseResult.GetValue(targetLengthKmOption),
            parseResult.GetValue(maxSpeedOption),
            parseResult.GetValue(minSpeedOption),
            parseResult.GetValue(maxAccelerationOption),
            parseResult.GetValue(pointIntervalOption),
            parseResult.GetValue(intervalVariationOption),
            parseResult.GetValue(updateIntervalSecondsOption),
            parseResult.GetValue(forceWriteOption)));

        return generateCommand;
    }

    private static Command GenerateBulkTracksCommand()
    {
        // Параметры командной строки
        Option<string> vehicleIdsFileOption = new Option<string>("--vehicle-ids-file", "-v") { Description = "Путь к файлу со списком ID автомобилей", Required = true };
        Option<DateTimeOffset> startDateOption = new Option<DateTimeOffset>("--start-date") { Description = "Дата начала периода генерации", Required = true };
        Option<DateTimeOffset> endDateOption = new Option<DateTimeOffset>("--end-date") { Description = "Дата окончания периода генерации", Required = true };
        Option<double> activeDaysRatioOption = new Option<double>("--active-days-ratio") { Description = "Доля активных дней (0.0-1.0)", Required = true };
        Option<int> batchSizeOption = new Option<int>("--batch-size") { Description = "Размер батча для обработки", Required = true };
        Option<double> centerLatOption = new Option<double>("--center-lat") { Description = "Широта центра области", Required = true };
        Option<double> centerLonOption = new Option<double>("--center-lon") { Description = "Долгота центра области", Required = true };
        Option<double> radiusKmOption = new Option<double>("--radius-km") { Description = "Радиус области в км", Required = true };
        Option<double> minAvgDailyDistanceOption = new Option<double>("--min-avg-daily-distance") { Description = "Минимальная средняя дневная дистанция в км", Required = true };
        Option<double> maxAvgDailyDistanceOption = new Option<double>("--max-avg-daily-distance") { Description = "Максимальная средняя дневная дистанция в км", Required = true };
        Option<double> maxSpeedOption = new Option<double>("--max-speed") { Description = "Максимальная скорость км/ч", Required = true };
        Option<double> minSpeedOption = new Option<double>("--min-speed") { Description = "Минимальная скорость км/ч", Required = true };
        Option<double> maxAccelerationOption = new Option<double>("--max-acceleration") { Description = "Максимальное ускорение км/ч²", Required = true };
        Option<int> pointIntervalOption = new Option<int>("--point-interval") { Description = "Интервал между точками в секундах", Required = true };
        Option<int> intervalVariationOption = new Option<int>("--interval-variation") { Description = "Разброс интервала в секундах", Required = true };
        Option<string> connectionStringBulkOption = new Option<string>("--connection-string", "-c") { Description = "Строка подключения к БД", Required = true };
        Option<string> apiKeyBulkOption = new Option<string>("--graphhopper-key", "-k") { Description = "API ключ GraphHopper", Required = true };

        // Команда массовой генерации треков
        Command bulkCommand = new Command("generate-bulk", "Массовая генерация треков")
        {
            vehicleIdsFileOption,
            startDateOption,
            endDateOption,
            activeDaysRatioOption,
            batchSizeOption,
            centerLatOption,
            centerLonOption,
            radiusKmOption,
            minAvgDailyDistanceOption,
            maxAvgDailyDistanceOption,
            maxSpeedOption,
            minSpeedOption,
            maxAccelerationOption,
            pointIntervalOption,
            intervalVariationOption,
            connectionStringBulkOption,
            apiKeyBulkOption
        };

        bulkCommand.SetAction(parseResult => GenerateTracksBulk(
            parseResult.GetValue(vehicleIdsFileOption)!,
            parseResult.GetValue(startDateOption),
            parseResult.GetValue(endDateOption),
            parseResult.GetValue(activeDaysRatioOption),
            parseResult.GetValue(minAvgDailyDistanceOption),
            parseResult.GetValue(maxAvgDailyDistanceOption),
            parseResult.GetValue(batchSizeOption),
            parseResult.GetValue(centerLatOption),
            parseResult.GetValue(centerLonOption),
            parseResult.GetValue(radiusKmOption),
            parseResult.GetValue(maxSpeedOption),
            parseResult.GetValue(minSpeedOption),
            parseResult.GetValue(maxAccelerationOption),
            parseResult.GetValue(pointIntervalOption),
            parseResult.GetValue(intervalVariationOption),
            parseResult.GetValue(connectionStringBulkOption)!,
            parseResult.GetValue(apiKeyBulkOption)!));

        return bulkCommand;
    }

    private static Command GenerateBulkRidesCommand()
    {
        // Параметры командной строки
        Option<DateTimeOffset> startDateOption = new Option<DateTimeOffset>("--start-date") { Description = "Дата начала периода генерации", Required = true };
        Option<DateTimeOffset> endDateOption = new Option<DateTimeOffset>("--end-date") { Description = "Дата окончания периода генерации", Required = true };
        Option<double> activeDaysRatioOption = new Option<double>("--active-days-ratio") { Description = "Доля активных дней (0.0-1.0)", Required = true };
        Option<int> averageRidesPerDayOption = new Option<int>("--average-rides-per-day") { Description = "Среднее количество поездок в день", Required = true };
        Option<int> minRideDurationMinutesOption = new Option<int>("--min-ride-duration-minutes") { Description = "Минимальная продолжительность поездки в минутах", Required = true };
        Option<int> maxRideDurationMinutesOption = new Option<int>("--max-ride-duration-minutes") { Description = "Максимальная продолжительность поездки в минутах", Required = true };
        Option<string> connectionStringBulkOption = new Option<string>("--connection-string", "-c") { Description = "Строка подключения к БД", Required = true };
        Option<string> apiKeyBulkOption = new Option<string>("--graphhopper-key", "-k") { Description = "API ключ GraphHopper", Required = true };

        // Команда массовой генерации поездок
        Command bulkCommand = new Command("generate-rides", "Массовая генерация поездок для всех автомобилей с треками в периоде")
        {
            startDateOption,
            endDateOption,
            activeDaysRatioOption,
            averageRidesPerDayOption,
            minRideDurationMinutesOption,
            maxRideDurationMinutesOption,
            connectionStringBulkOption,
            apiKeyBulkOption
        };

        bulkCommand.SetAction(parseResult => GenerateRidesBulk(
            parseResult.GetValue(startDateOption),
            parseResult.GetValue(endDateOption),
            parseResult.GetValue(activeDaysRatioOption),
            parseResult.GetValue(averageRidesPerDayOption),
            parseResult.GetValue(minRideDurationMinutesOption),
            parseResult.GetValue(maxRideDurationMinutesOption),
            parseResult.GetValue(connectionStringBulkOption)!,
            parseResult.GetValue(apiKeyBulkOption)!));

        return bulkCommand;
    }

    private static Command GenerateTemplateCommand()
    {
        // Параметры командной строки
        Option<string> outputOption = new Option<string>("--output", "-o") { Description = "Путь к выходному GeoJSON файлу", Required = true };
        Option<string> connectionStringOption = new Option<string>("--connection-string", "-c") { Description = "Строка подключения к БД", Required = true };
        Option<string> graphHopperApiKeyOption = new Option<string>("--graphhopper-key", "-k") { Description = "API ключ GraphHopper", Required = true };

        // Параметры трека
        Option<double> centerLatOption = new Option<double>("--center-lat", "-lat") { Description = "Широта центра области", Required = true };
        Option<double> centerLonOption = new Option<double>("--center-lon", "-lon") { Description = "Долгота центра области", Required = true };
        Option<double> minTargetLengthOption = new Option<double>("--min-target-length-km") { Description = "Минимальная целевая длина трека в км", Required = true };
        Option<double> maxTargetLengthOption = new Option<double>("--max-target-length-km") { Description = "Максимальная целевая длина трека в км", Required = true };
        Option<double> maxSpeedOption = new Option<double>("--max-speed", "-max-s") { Description = "Максимальная скорость км/ч", Required = true };
        Option<double> minSpeedOption = new Option<double>("--min-speed", "-min-s") { Description = "Минимальная скорость км/ч", Required = true };
        Option<double> maxAccelerationOption = new Option<double>("--max-acceleration", "-a") { Description = "Максимальное ускорение км/ч²", Required = true };

        // Шаг точек с разбросом
        Option<int> pointIntervalOption = new Option<int>("--point-interval", "-p") { Description = "Интервал между точками в секундах", Required = true };
        Option<int> intervalVariationOption = new Option<int>("--interval-variation", "-pv") { Description = "Разброс интервала между точками в секундах", Required = true };

        // Радиус области
        Option<double> radiusKmOption = new Option<double>("--radius-km", "-r") { Description = "Радиус области в км", Required = true };

        // Команда генерации шаблона
        Command templateCommand = new Command("generate-template", "Генерация шаблона трека в GeoJSON")
        {
            outputOption,
            connectionStringOption,
            graphHopperApiKeyOption,
            centerLatOption,
            centerLonOption,
            minTargetLengthOption,
            maxTargetLengthOption,
            maxSpeedOption,
            minSpeedOption,
            maxAccelerationOption,
            pointIntervalOption,
            intervalVariationOption,
            radiusKmOption
        };

        templateCommand.SetAction(parseResult => GenerateTemplateAsync(
            parseResult.GetValue(outputOption)!,
            parseResult.GetValue(connectionStringOption)!,
            parseResult.GetValue(graphHopperApiKeyOption)!,
            parseResult.GetValue(centerLatOption),
            parseResult.GetValue(centerLonOption),
            parseResult.GetValue(minTargetLengthOption),
            parseResult.GetValue(maxTargetLengthOption),
            parseResult.GetValue(maxSpeedOption),
            parseResult.GetValue(minSpeedOption),
            parseResult.GetValue(maxAccelerationOption),
            parseResult.GetValue(pointIntervalOption),
            parseResult.GetValue(intervalVariationOption),
            parseResult.GetValue(radiusKmOption)));

        return templateCommand;
    }

    private static Command GenerateFromTemplatesCommand()
    {
        // Параметры командной строки
        Option<string> templatesDirOption = new Option<string>("--templates-dir", "-t") { Description = "Директория с шаблонами треков", Required = true };
        Option<DateTimeOffset> startDateOption = new Option<DateTimeOffset>("--start-date") { Description = "Дата начала периода генерации", Required = true };
        Option<DateTimeOffset> endDateOption = new Option<DateTimeOffset>("--end-date") { Description = "Дата окончания периода генерации", Required = true };
        Option<double> activeDaysRatioOption = new Option<double>("--active-days-ratio") { Description = "Доля активных дней (0.0-1.0)", Required = true };
        Option<double> minAvgDailyDistanceOption = new Option<double>("--min-avg-daily-distance") { Description = "Минимальная средняя дневная дистанция в км", Required = true };
        Option<double> maxAvgDailyDistanceOption = new Option<double>("--max-avg-daily-distance") { Description = "Максимальная средняя дневная дистанция в км", Required = true };
        Option<int> batchSizeOption = new Option<int>("--batch-size") { Description = "Размер батча для обработки автомобилей", Required = true };
        Option<string> connectionStringOption = new Option<string>("--connection-string", "-c") { Description = "Строка подключения к БД", Required = true };

        // Команда генерации треков из шаблонов
        Command fromTemplatesCommand = new Command("generate-from-templates", "Генерация треков для всех автомобилей из шаблонов")
        {
            templatesDirOption,
            startDateOption,
            endDateOption,
            activeDaysRatioOption,
            minAvgDailyDistanceOption,
            maxAvgDailyDistanceOption,
            batchSizeOption,
            connectionStringOption
        };

        fromTemplatesCommand.SetAction(parseResult => GenerateFromTemplatesAsync(
            parseResult.GetValue(templatesDirOption)!,
            parseResult.GetValue(startDateOption),
            parseResult.GetValue(endDateOption),
            parseResult.GetValue(activeDaysRatioOption),
            parseResult.GetValue(minAvgDailyDistanceOption),
            parseResult.GetValue(maxAvgDailyDistanceOption),
            parseResult.GetValue(batchSizeOption),
            parseResult.GetValue(connectionStringOption)!));

        return fromTemplatesCommand;
    }

    static async Task GenerateTemplateAsync(
        string outputPath,
        string connectionString,
        string apiKey,
        double centerLat,
        double centerLon,
        double minTargetLengthKm,
        double maxTargetLengthKm,
        double maxSpeed,
        double minSpeed,
        double maxAcceleration,
        int pointInterval,
        int intervalVariation,
        double radiusKm)
    {
        Console.WriteLine("Генерация шаблона трека...");
        Console.WriteLine($"Центр: {centerLat}, {centerLon}");
        Console.WriteLine($"Диапазон длины: {minTargetLengthKm:F1}-{maxTargetLengthKm:F1} км");
        Console.WriteLine($"Радиус: {radiusKm} км");
        Console.WriteLine($"Скорость: {minSpeed}-{maxSpeed} км/ч");
        Console.WriteLine($"Ускорение: {maxAcceleration} км/ч²");
        Console.WriteLine($"Интервал точек: {pointInterval} ±{intervalVariation} сек");
        Console.WriteLine();

        // Настройка DI контейнера
        ServiceCollection services = new ServiceCollection();
        ConfigureServices(services, connectionString, apiKey);

        await using ServiceProvider serviceProvider = services.BuildServiceProvider();

        // Создание геометрической точки центра
        GeometryFactory geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        Point centerPoint = geometryFactory.CreatePoint(new Coordinate(centerLon, centerLat));

        // Генерация случайной целевой длины
        Random random = new Random();
        double targetLengthKm = random.NextDouble() * (maxTargetLengthKm - minTargetLengthKm) + minTargetLengthKm;

        // Параметры генерации трека
        TrackGenerationOptions options = new TrackGenerationOptions
        {
            CenterPoint = centerPoint,
            RadiusKm = radiusKm,
            TargetLengthKm = targetLengthKm,
            MaxSpeedKmH = maxSpeed,
            MinSpeedKmH = minSpeed,
            MaxAccelerationKmH2 = maxAcceleration,
            PointInterval = TimeSpan.FromSeconds(pointInterval),
            IntervalVariation = TimeSpan.FromSeconds(intervalVariation),
            StartTime = DateTimeOffset.UtcNow
        };

        // Генерация трека
        var trackService = serviceProvider.GetRequiredService<ITrackGenerationService>();
        Track track = await trackService.GenerateTrackAsync(options);

        // Конвертация в GeoJSON с относительными временными метками
        string geoJson = ConvertTrackToGeoJson(geometryFactory, track);

        // Сохранение в файл
        await File.WriteAllTextAsync(outputPath, geoJson);

        // Вывод статистики
        var firstPoint = track.Points.First();
        var lastPoint = track.Points.Last();
        var duration = lastPoint.Timestamp - firstPoint.Timestamp;

        Console.WriteLine($"Сгенерировано {track.Points.Count} точек");
        Console.WriteLine($"Общая длина: {targetLengthKm:F2} км");
        Console.WriteLine($"Время: {duration.TotalMinutes:F1} минут");
        Console.WriteLine($"Сохранено в {outputPath}");
    }

    static string ConvertTrackToGeoJson(GeometryFactory factory, Track track)
    {
        DateTimeOffset startTime = track.Points.First().Timestamp;
        List<Feature> features = new List<Feature>();

        foreach (var point in track.Points)
        {
            int relativeSeconds = (int)(point.Timestamp - startTime).TotalSeconds;

            Feature feature = new Feature
            {
                Geometry = point.Location,
                Attributes = new AttributesTable
                {
                    { "timestamp", relativeSeconds }
                }
            };

            features.Add(feature);
        }

        FeatureCollection featureCollection = new FeatureCollection(features);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var geoJsonFactory = new GeoJsonConverterFactory(factory);
        options.Converters.Add(geoJsonFactory);

        string geoJson = JsonSerializer.Serialize(featureCollection, options);

        return geoJson;
    }

    static async Task GenerateRidesBulk(
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        double activeDaysRatio,
        int averageRidesPerDay,
        int minRideDurationMinutes,
        int maxRideDurationMinutes,
        string connectionString,
        string apiKey)
    {
        // 1. Получить vehicle IDs из базы данных - автомобили, у которых есть треки в заданном периоде
        ServiceCollection tempServices = new ServiceCollection();
        tempServices.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, x => x.UseNetTopologySuite())
                   .UseSnakeCaseNamingConvention());
        await using ServiceProvider tempProvider = tempServices.BuildServiceProvider();

        using var scope = tempProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        List<Guid> vehicleIds = await context.VehicleGeoTimePoints
            .Where(p => p.Time >= startDate.ToUniversalTime() && p.Time <= endDate.ToUniversalTime())
            .Select(p => p.Vehicle.Id)
            .Distinct()
            .ToListAsync();

        Console.WriteLine("Bulk Ride Generation");
        Console.WriteLine($"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
        Console.WriteLine($"Active days ratio: {activeDaysRatio:P0}");
        Console.WriteLine($"Average rides per day: {averageRidesPerDay}");
        Console.WriteLine($"Ride duration: {minRideDurationMinutes}-{maxRideDurationMinutes} min");
        Console.WriteLine();

        Console.WriteLine($"Found {vehicleIds.Count} vehicles with tracks in the period");

        if (vehicleIds.Count == 0)
        {
            Console.WriteLine("No vehicles with tracks found in the specified period. Skipping ride generation.");
            return;
        }

        // Настройка DI
        ServiceCollection services = new ServiceCollection();
        ConfigureServicesForBulk(services, connectionString, apiKey);
        await using ServiceProvider serviceProvider = services.BuildServiceProvider();

        // Создать опции
        BulkRideGenerationOptions options = new BulkRideGenerationOptions
        {
            StartDate = startDate,
            EndDate = endDate,
            ActiveDaysRatio = activeDaysRatio,
            AverageRidesPerDay = averageRidesPerDay,
            MinRideDurationMinutes = minRideDurationMinutes,
            MaxRideDurationMinutes = maxRideDurationMinutes,
            ConnectionString = connectionString,
            GraphHopperApiKey = apiKey
        };

        // Запустить генерацию
        BulkRideGenerationService bulkService = serviceProvider.GetRequiredService<BulkRideGenerationService>();
        await bulkService.GenerateRidesForAllVehiclesAsync(vehicleIds, options);
    }

    static async Task GenerateTrackAsync(Guid vehicleId,
        string connectionString, 
        string apiKey,
        double centerLat,
        double centerLon,
        double radiusKm, 
        double targetLengthKm, 
        double maxSpeed,
        double minSpeed,
        double maxAcceleration,
        int pointInterval, 
        int intervalVariation, 
        int updateIntervalSeconds,
        bool force)
    {
        Console.WriteLine("Генератор треков автомобилей");
        Console.WriteLine($"Автомобиль: {vehicleId}");
        Console.WriteLine($"Центр: {centerLat}, {centerLon}");
        Console.WriteLine($"Радиус: {radiusKm} км");
        Console.WriteLine($"Длина трека: {targetLengthKm} км");
        Console.WriteLine($"Максимальная скорость: {maxSpeed} км/ч");
        Console.WriteLine($"Максимальное ускорение: {maxAcceleration} км/ч²");
        Console.WriteLine($"Интервал точек: {pointInterval} ±{intervalVariation} сек");
        Console.WriteLine($"Интервал обновления: {updateIntervalSeconds} сек");
        Console.WriteLine();

        // Настройка DI контейнера
        ServiceCollection services = new ServiceCollection();
        ConfigureServices(services, connectionString, apiKey);

        await using ServiceProvider serviceProvider = services.BuildServiceProvider();

        // Создание геометрической точки центра
        GeometryFactory geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        Point centerPoint = geometryFactory.CreatePoint(new Coordinate(centerLon, centerLat));

        // Параметры генерации трека
        TrackGenerationOptions options = new TrackGenerationOptions
        {
            CenterPoint = centerPoint,
            RadiusKm = radiusKm,
            TargetLengthKm = targetLengthKm, 
            MaxSpeedKmH = maxSpeed,
            MinSpeedKmH = minSpeed,
            MaxAccelerationKmH2 = maxAcceleration,
            PointInterval = TimeSpan.FromSeconds(pointInterval),
            IntervalVariation = TimeSpan.FromSeconds(intervalVariation),
            StartTime = DateTimeOffset.UtcNow
        };

        try
        {
            // Запуск сервиса генерации треков

            if (force)
            {
                TrackForceTimeWriterService trackRealTimeWriter = serviceProvider.GetRequiredService<TrackForceTimeWriterService>();
                await trackRealTimeWriter.GenerateAndWriteTrackAsync(vehicleId, options, updateIntervalSeconds);
            }
            else
            {
                TrackRealTimeWriterService trackRealTimeWriter = serviceProvider.GetRequiredService<TrackRealTimeWriterService>();
                await trackRealTimeWriter.GenerateAndWriteTrackAsync(vehicleId, options, updateIntervalSeconds);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Подробности: {ex.InnerException.Message}");
            }
        }
    }

    static async Task GenerateTracksBulk(
        string vehicleIdsFile,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        double activeDaysRatio,
        double minAvgDailyDistance,
        double maxAvgDailyDistance,
        int batchSize,
        double centerLat,
        double centerLon,
        double radiusKm,
        double maxSpeed,
        double minSpeed,
        double maxAcceleration,
        int pointInterval,
        int intervalVariation,
        string connectionString,
        string apiKey)
    {
        Console.WriteLine("🚗 Bulk Track Generation");
        Console.WriteLine($"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
        Console.WriteLine($"Active days ratio: {activeDaysRatio:P0}");
        Console.WriteLine($"Avg daily distance: {minAvgDailyDistance:F1}-{maxAvgDailyDistance:F1} km");
        Console.WriteLine($"Center: {centerLat:F6}, {centerLon:F6}");
        Console.WriteLine($"Radius: {radiusKm} km");
        Console.WriteLine($"Speed: {minSpeed}-{maxSpeed} km/h");
        Console.WriteLine($"Acceleration: {maxAcceleration} km/h²");
        Console.WriteLine($"Point interval: {pointInterval}s ±{intervalVariation}s");
        Console.WriteLine();

        // 1. Прочитать vehicle IDs из файла
        List<Guid> vehicleIds = File.ReadAllLines(vehicleIdsFile)
            .Select(line => Guid.Parse(line.Trim()))
            .ToList();

        Console.WriteLine($"📋 Обработка {vehicleIds.Count} vehicle IDs");

        // 2. Настройка DI
        ServiceCollection services = new ServiceCollection();
        ConfigureServicesForBulk(services, connectionString, apiKey);
        await using ServiceProvider serviceProvider = services.BuildServiceProvider();

        // 3. Создать центральную точку
        GeometryFactory geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        Point centerPoint = geometryFactory.CreatePoint(new Coordinate(centerLon, centerLat));

        // 4. Создать опции
        BulkTrackGenerationOptions options = new BulkTrackGenerationOptions
        {
            StartDate = startDate,
            EndDate = endDate,
            ActiveDaysRatio = activeDaysRatio,
            MinAvgDailyDistanceKm = minAvgDailyDistance,
            MaxAvgDailyDistanceKm = maxAvgDailyDistance,
            BatchSize = batchSize,
            CenterPoint = centerPoint,
            RadiusKm = radiusKm,
            MaxSpeedKmH = maxSpeed,
            MinSpeedKmH = minSpeed,
            MaxAccelerationKmH2 = maxAcceleration,
            PointInterval = TimeSpan.FromSeconds(pointInterval),
            IntervalVariation = TimeSpan.FromSeconds(intervalVariation),
            ConnectionString = connectionString,
            GraphHopperApiKey = apiKey
        };

        // 5. Запустить генерацию
        BulkTrackGenerationService bulkService = serviceProvider.GetRequiredService<BulkTrackGenerationService>();
        await bulkService.GenerateTracksForAllVehiclesAsync(vehicleIds, options);
    }

    static void ConfigureServicesForBulk(
        ServiceCollection services,
        string connectionString,
        string apiKey)
    {
        services.AddLogging(builder => builder.AddConsole());

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, x => x.UseNetTopologySuite())
                   .UseSnakeCaseNamingConvention());

        services.AddSingleton<LocalIcuTimezoneService>();

        services.Configure<GraphHopperOptions>(options =>
        {
            options.ApiKey = apiKey;
        });

        services.AddHttpClient<IGraphHopperApiClient, GraphHopperApiClient>();
        services.AddScoped<IRouteGenerationService, RouteGenerationService>();
        services.AddScoped<ITrackGenerationService, TrackGenerationService>();
        services.AddScoped<BulkTrackGenerationService>();
        services.AddScoped<BulkRideGenerationService>();
    }

    static void ConfigureServices(ServiceCollection services, string connectionString, string apiKey)
    {
        // Логирование
        services.AddLogging(builder => builder.AddConsole());

        // База данных
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, x => x.UseNetTopologySuite())
                   .UseSnakeCaseNamingConvention());

        services.AddSingleton<LocalIcuTimezoneService>();

        // GraphHopper конфигурация
        services.Configure<GraphHopperOptions>(options =>
        {
            options.ApiKey = apiKey;
        });

        // HTTP клиент
        services.AddHttpClient<IGraphHopperApiClient, GraphHopperApiClient>();

        // Сервисы генерации
        services.AddScoped<IRouteGenerationService, RouteGenerationService>();
        services.AddScoped<ITrackGenerationService, TrackGenerationService>();
        services.AddScoped<TrackRealTimeWriterService>();
        services.AddScoped<TrackForceTimeWriterService>();
    }

    static async Task GenerateFromTemplatesAsync(
        string templatesDir,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        double activeDaysRatio,
        double minAvgDailyDistance,
        double maxAvgDailyDistance,
        int batchSize,
        string connectionString)
    {
        Console.WriteLine("🚗 Generate Tracks from Templates");
        Console.WriteLine($"Templates directory: {templatesDir}");
        Console.WriteLine($"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
        Console.WriteLine($"Active days ratio: {activeDaysRatio:P0}");
        Console.WriteLine($"Avg daily distance: {minAvgDailyDistance:F1}-{maxAvgDailyDistance:F1} km");
        Console.WriteLine();

        // 1. Load templates
        var templates = await LoadTemplatesAsync(templatesDir);
        if (templates.Count == 0)
        {
            Console.WriteLine("❌ No templates found!");
            return;
        }

        Console.WriteLine($"📋 Loaded {templates.Count} templates");

        // 2. Setup DI
        ServiceCollection services = new ServiceCollection();
        ConfigureServicesForBulk(services, connectionString, "dummy"); // No API key needed for template-based generation
        await using ServiceProvider serviceProvider = services.BuildServiceProvider();

        // 3. Get all vehicles
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var vehicles = await context.Vehicles.ToListAsync();

        Console.WriteLine($"📋 Processing {vehicles.Count} vehicles in batches");

        // 4. Generate tracks for vehicles in batches
        var random = new Random();
        var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);

        for (int i = 0; i < vehicles.Count; i += batchSize)
        {
            var batch = vehicles.Skip(i).Take(batchSize).ToList();
            Console.WriteLine($"Processing batch {i / batchSize + 1} ({batch.Count} vehicles)...");

            foreach (var vehicle in batch)
            {
                Console.WriteLine($"  Processing vehicle {vehicle.Id} ({vehicle.VinNumber})");

                // Generate track for this vehicle using templates
                var trackPoints = await GenerateTrackFromTemplatesAsync(vehicle, templates, startDate, endDate,
                    activeDaysRatio, minAvgDailyDistance, maxAvgDailyDistance, random, geometryFactory);

                // Save track points to database
                await SaveTrackPointsAsync(context, vehicle, trackPoints);
            }

            Console.WriteLine($"  Batch {i / batchSize + 1} completed");
        }

        Console.WriteLine("✅ Track generation from templates completed!");
    }

    static async Task<List<TrackTemplate>> LoadTemplatesAsync(string templatesDir)
    {
        var templates = new List<TrackTemplate>();
        var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        Console.WriteLine($"Looking for templates in: {Path.GetFullPath(templatesDir)}");
        Console.WriteLine($"Directory exists: {Directory.Exists(templatesDir)}");
        var jsonFiles = Directory.GetFiles(templatesDir, "*.json");
        Console.WriteLine($"Found {jsonFiles.Length} json files");

        foreach (var file in jsonFiles)
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                var template = ParseGeoJsonTemplate(json, geometryFactory);
                if (template != null)
                {
                    templates.Add(template);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to load template {file}: {ex.Message}");
            }
        }

        return templates;
    }

    static TrackTemplate? ParseGeoJsonTemplate(string json, GeometryFactory geometryFactory)
    {
        try
        {
            Console.WriteLine("Parsing template JSON...");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var geoJsonFactory = new GeoJsonConverterFactory(geometryFactory);
            options.Converters.Add(geoJsonFactory);

            var featureCollection = JsonSerializer.Deserialize<FeatureCollection>(json, options);
            if (featureCollection == null || featureCollection.Count == 0)
            {
                Console.WriteLine("FeatureCollection is null or empty");
                return null;
            }

            Console.WriteLine($"Found {featureCollection.Count} features");
            var points = new List<GeoTimePoint>();
            foreach (var feature in featureCollection)
            {
                if (feature.Geometry is Point point)
                {
                    try
                    {
                        var timestamp = Convert.ToInt32(feature.Attributes["timestamp"]);
                        points.Add(new GeoTimePoint
                        {
                            Location = point,
                            RelativeSeconds = timestamp
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing feature: {ex.Message}");
                        // Skip features without timestamp
                    }
                }
                else
                {
                    Console.WriteLine($"Feature geometry is not a Point: {feature.Geometry?.GeometryType}");
                }
            }

            Console.WriteLine($"Parsed {points.Count} points");
            if (points.Count == 0)
                return null;

            return new TrackTemplate { Points = points };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing template: {ex.Message}");
            return null;
        }
    }

    static async Task<List<VehicleGeoTimePoint>> GenerateTrackFromTemplatesAsync(
        Vehicle vehicle,
        List<TrackTemplate> templates,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        double activeDaysRatio,
        double minAvgDailyDistance,
        double maxAvgDailyDistance,
        Random random,
        GeometryFactory geometryFactory)
    {
        var trackPoints = new List<VehicleGeoTimePoint>();
        var currentDate = startDate;

        while (currentDate <= endDate)
        {
            // Check if vehicle is active on this day
            if (random.NextDouble() > activeDaysRatio)
            {
                currentDate = currentDate.AddDays(1);
                continue;
            }

            // Generate daily distance
            var dailyDistance = random.NextDouble() * (maxAvgDailyDistance - minAvgDailyDistance) + minAvgDailyDistance;

            // Generate track for this day
            var dailyPoints = await GenerateDailyTrackFromTemplatesAsync(vehicle, templates,
                currentDate, dailyDistance, random, geometryFactory);

            trackPoints.AddRange(dailyPoints);
            currentDate = currentDate.AddDays(1);
        }

        return trackPoints;
    }

    static async Task<List<VehicleGeoTimePoint>> GenerateDailyTrackFromTemplatesAsync(
        Vehicle vehicle,
        List<TrackTemplate> templates,
        DateTimeOffset date,
        double targetDistanceKm,
        Random random,
        GeometryFactory geometryFactory)
    {
        var points = new List<VehicleGeoTimePoint>();
        double currentDistance = 0;
        Point? lastEndPoint = null;
        TrackTemplate? previousTemplate = null;

        while (currentDistance < targetDistanceKm)
        {
            TrackTemplate selectedTemplate;
            int startIndex;

            if (lastEndPoint == null)
            {
                // First piece - select random template and random start
                selectedTemplate = templates[random.Next(templates.Count)];
                startIndex = random.Next(selectedTemplate.Points.Count);
            }
            else
            {
                // Find 3 closest templates to the last end point, excluding previous
                var closestTemplates = templates
                    .Where(t => t != previousTemplate)
                    .Select(t => new { Template = t, Distance = CalculateDistance(lastEndPoint, t.Points.First().Location) })
                    .OrderBy(x => x.Distance)
                    .Take(3)
                    .ToList();

                // Select random from closest
                selectedTemplate = closestTemplates[random.Next(closestTemplates.Count)].Template;

                // Find best connection point (closest to lastEndPoint)
                startIndex = selectedTemplate.Points
                    .Select((p, i) => new { Point = p, Index = i, Distance = CalculateDistance(lastEndPoint, p.Location) })
                    .OrderBy(x => x.Distance)
                    .First().Index;
            }

            // Take a random piece from the selected template (at least 10 points, at most 50% of template)
            int remainingPoints = selectedTemplate.Points.Count - startIndex;
            int halfTemplate = selectedTemplate.Points.Count / 2;
            // int maxPieceLength = remainingPoints;
            // int minPieceLength = Math.Max(1, (int)(remainingPoints * 0.5));
            // int pieceLength = random.Next(minPieceLength, maxPieceLength + 1);

            List<GeoTimePoint> piece;
            if (remainingPoints < halfTemplate) {
                // Go backward
                var backwardLength = Math.Min(selectedTemplate.Points.Count, startIndex + 1);
                var backwardStart = Math.Max(0, startIndex - backwardLength + 1);
                piece = selectedTemplate.Points.Skip(backwardStart).Take(backwardLength).ToList();
            } else {
                // Normal forward piece
                var maxPieceLength = remainingPoints;
                var minPieceLength = Math.Max(10, (int)(remainingPoints * 0.5));
                var pieceLength = random.Next(minPieceLength, maxPieceLength + 1);
                piece = selectedTemplate.Points.Skip(startIndex).Take(pieceLength).ToList();
            }

            // Extract piece
            //var piece = selectedTemplate.Points.Skip(startIndex).Take(pieceLength).ToList();

            // Convert to VehicleGeoTimePoint with absolute timestamps
            var piecePoints = ConvertTemplatePieceToGeoPoints(vehicle, piece, date, points.Count > 0 ? points.Last().Time.Value : date);

            points.AddRange(piecePoints);

            // Update distance and last end point
            if (piece.Count > 1)
            {
                var pieceDistance = CalculatePieceDistance(piece);
                currentDistance += pieceDistance;
                lastEndPoint = piece.Last().Location;
            }
            else if (piece.Count == 1)
            {
                lastEndPoint = piece.First().Location;
            }

            // Update previous template
            previousTemplate = selectedTemplate;
        }

        return points;
    }

    static double CalculateDistance(Point p1, Point p2)
    {
        // Haversine distance approximation
        const double R = 6371; // Earth's radius in km
        var dLat = (p2.Y - p1.Y) * Math.PI / 180;
        var dLon = (p2.X - p1.X) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(p1.Y * Math.PI / 180) * Math.Cos(p2.Y * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    static double CalculatePieceDistance(List<GeoTimePoint> piece)
    {
        double distance = 0;
        for (int i = 1; i < piece.Count; i++)
        {
            distance += CalculateDistance(piece[i - 1].Location, piece[i].Location);
        }
        return distance;
    }

    static List<VehicleGeoTimePoint> ConvertTemplatePieceToGeoPoints(
    Vehicle vehicle,
    List<GeoTimePoint> piece,
    DateTimeOffset baseDate,
    DateTimeOffset lastTimestamp)
    {
        var points = new List<VehicleGeoTimePoint>();

        // Нормализуем время: вычитаем время первой точки куска
        var baseRelativeSeconds = piece.First().RelativeSeconds;

        foreach (var templatePoint in piece)
        {
            // Вычисляем смещение от начала куска
            var timeOffset = templatePoint.RelativeSeconds - baseRelativeSeconds;
            var pointTime = lastTimestamp.AddSeconds(timeOffset);

            var result = VehicleGeoTimePoint.Create(
                Guid.NewGuid(),
                vehicle,
                templatePoint.Location,
                new CarPark.Shared.DateTimes.UtcDateTimeOffset(pointTime)
            );

            if (result.IsSuccess)
            {
                points.Add(result.Value);
            }
        }

        return points;
    }

    static async Task SaveTrackPointsAsync(ApplicationDbContext context, Vehicle vehicle, List<VehicleGeoTimePoint> points)
    {
        if (points.Count == 0)
            return;

        // Save in batches to avoid memory issues
        const int batchSize = 1000;
        for (int i = 0; i < points.Count; i += batchSize)
        {
            var batch = points.Skip(i).Take(batchSize).ToList();
            await context.VehicleGeoTimePoints.AddRangeAsync(batch);
            await context.SaveChangesAsync();
        }
    }

    class TrackTemplate
    {
        public List<GeoTimePoint> Points { get; set; } = new();
    }

    class GeoTimePoint
    {
        public Point Location { get; set; } = null!;
        public int RelativeSeconds { get; set; }
    }
}

