using CarPark.Data;
using CarPark.Services.TimeZones;
using CarPark.TrackGenerator.GraphHopper;
using CarPark.TrackGenerator.Interfaces;
using CarPark.TrackGenerator.Models;
using CarPark.TrackGenerator.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using System.CommandLine;

namespace CarPark.TrackGenerator;

    internal class Program
    {

    static int Main(string[] args)
    {
        RootCommand rootCommand = new RootCommand("Генератор треков автомобилей в реальном времени");

        // Параметры командной строки
        Option<Guid> vehicleIdOption = new Option<Guid>("--vehicle-id", "-v") 
        { 
            Description = "ID автомобиля для генерации трека", 
            Required = true 
        };

        Option<string> connectionStringOption = new Option<string>("--connection-string", "-c") 
        { 
            Description = "Строка подключения к БД", 
            Required = true 
        };

        Option<string> graphHopperApiKeyOption = new Option<string>("--api-key", "-k") 
        { 
            Description = "API ключ GraphHopper", 
            Required = true 
        };

        // Параметры трека
        Option<double> centerLatOption = new Option<double>("--center-lat", "-lat") 
        { 
            Description = "Широта центра области (например, 55.7558 для Москвы)", 
            Required = true 
        };

        Option<double> centerLonOption = new Option<double>("--center-lon", "-lon") 
        { 
            Description = "Долгота центра области (например, 37.6176 для Москвы)", 
            Required = true 
        };

        Option<bool> forceWriteOption = new Option<bool>("--force-write", "-for")
        {
            Description = "Сразу записать весь трек (в т.ч. точки, пройденные в будущем)",
            Required = true
        };

        Option<double> radiusKmOption = new Option<double>("--radius-km", "-r") { Description = "Радиус области в км", Required = true };
        Option<double> targetLengthKmOption = new Option<double>("--target-length-km", "-l") { Description = "Целевая длина трека в км", Required = true };
        Option<double> maxSpeedOption = new Option<double>("--max-speed", "-min-s") { Description = "Максимальная скорость км/ч", Required = true };
        Option<double> minSpeedOption = new Option<double>("--min-speed", "-max-ss") { Description = "Максимальная скорость км/ч", Required = true };
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
            parseResult.GetValue(forceWriteOption)).GetAwaiter().GetResult());

        rootCommand.Add(generateCommand);

        return rootCommand.Parse(args).Invoke();
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
}

