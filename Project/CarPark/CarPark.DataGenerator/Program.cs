using CarPark.Data;
using CarPark.Models.Drivers;
using CarPark.Models.Models;
using CarPark.Models.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.CommandLine;
using System.Text.Json;

namespace CarPark.DataGenerator;

internal class Program
{
    static int Main(string[] args)
    {
        RootCommand rootCommand = new RootCommand("Утилита генерации тестовых данных для CarPark");

        // Команда vehicles-drivers
        Option<string> enterpriseIdsOption = new Option<string>("--enterprise-ids", "-e") { Description = "Список ID предприятий через запятую", Required = true };
        Option<int> vehiclesPerEnterpriseOption = new Option<int>("--vehicles-per-enterprise", "-v") { Description = "Количество машин на предприятие", Required = true };
        Option<int> driversPerEnterpriseOption = new Option<int>("--drivers-per-enterprise", "-d") { Description = "Количество водителей на предприятие", Required = true };
        Option<string> outputVehiclesOption = new Option<string>("--output", "-o") { Description = "Путь к выходному JSON файлу" };
        Option<string> connectionStringVehiclesOption = new Option<string>("--connection-string", "-c") { Description = "Строка подключения к БД", Required = true };

        Command generateCommand = new Command("generate", "Генерация данных");
        
        Command vehiclesDriversCommand = new Command("vehicles-drivers", "Генерация автомобилей и водителей")
        {
            enterpriseIdsOption,
            vehiclesPerEnterpriseOption,
            driversPerEnterpriseOption,
            outputVehiclesOption,
            connectionStringVehiclesOption
        };

        vehiclesDriversCommand.SetAction(parseResult => GenerateVehiclesAndDrivers(
            parseResult.GetValue(enterpriseIdsOption)!,
            parseResult.GetValue(vehiclesPerEnterpriseOption),
            parseResult.GetValue(driversPerEnterpriseOption),
            parseResult.GetValue(outputVehiclesOption),
            parseResult.GetValue(connectionStringVehiclesOption)!));

        generateCommand.Add(vehiclesDriversCommand);

        rootCommand.Add(generateCommand);

        return rootCommand.Parse(args).Invoke();
    }

    static ApplicationDbContext CreateDbContext(string connectionString)
    {
        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    static List<Model> LoadModelsFromDatabase(ApplicationDbContext context)
    {
        List<Model> models = context.Models.ToList();
        Console.WriteLine($"Загружено {models.Count} моделей из базы данных");
        return models;
    }

    static List<int> ValidateEnterprisesInDatabase(List<int> enterpriseIds, ApplicationDbContext context)
    {
        List<int> existingEnterpriseIds = context.Enterprises
            .Where(e => enterpriseIds.Contains(e.Id))
            .Select(e => e.Id)
            .ToList();

        List<int> missingIds = enterpriseIds.Except(existingEnterpriseIds).ToList();
        if (missingIds.Any())
        {
            Console.WriteLine($"Предупреждение: Следующие предприятия не найдены в БД: {string.Join(", ", missingIds)}");
        }

        Console.WriteLine($"Найдено {existingEnterpriseIds.Count} существующих предприятий из {enterpriseIds.Count} запрошенных");
        return existingEnterpriseIds;
    }

    static void GenerateVehiclesAndDrivers(string enterpriseIdsString, int vehiclesPerEnterprise, int driversPerEnterprise, string? outputPath, string connectionString)
    {
        // Парсим список ID предприятий
        List<int> enterpriseIds = enterpriseIdsString.Split(',')
            .Select(id => int.Parse(id.Trim()))
            .ToList();

        Console.WriteLine($"Генерация автомобилей и водителей:");
        Console.WriteLine($"- Запрошено предприятий: {enterpriseIds.Count}");
        Console.WriteLine($"- Машин на предприятие: {vehiclesPerEnterprise}");
        Console.WriteLine($"- Водителей на предприятие: {driversPerEnterprise}");
        
        if (!string.IsNullOrEmpty(outputPath))
        {
            Console.WriteLine($"- Выходной файл: {outputPath}");
        }
        else if (!string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine($"- Сохранение в базу данных");
        }
        Console.WriteLine();

        // Создаем контекст БД
        using ApplicationDbContext dbContext = CreateDbContext(connectionString);

        using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

        // Загружаем модели из БД
        List<Model> models = LoadModelsFromDatabase(dbContext);
        if (!models.Any())
        {
            Console.WriteLine("Ошибка: Не удалось загрузить модели из базы данных");
            return;
        }

        // Проверяем существование предприятий в БД
        List<int> existingEnterpriseIds = ValidateEnterprisesInDatabase(enterpriseIds, dbContext);
        if (!existingEnterpriseIds.Any())
        {
            Console.WriteLine("Ошибка: Не найдено ни одного существующего предприятия");
            return;
        }

        DataGenerator generator = new DataGenerator();
        
        // Генерируем автомобили и водителей для каждого предприятия
        List<Vehicle> allVehicles = new List<Vehicle>();
        List<Driver> allDrivers = new List<Driver>();
        
        foreach (int enterpriseId in existingEnterpriseIds)
        {
            // Генерируем автомобили для предприятия
            List<Vehicle> enterpriseVehicles = generator.GenerateVehicles(enterpriseId, models)
                .Take(vehiclesPerEnterprise)
                .ToList();

            // Генерируем водителей для предприятия
            List<Driver> enterpriseDrivers = generator.GenerateDrivers(enterpriseId)
                .Take(driversPerEnterprise)
                .ToList();
            
            allVehicles.AddRange(enterpriseVehicles);
            allDrivers.AddRange(enterpriseDrivers);
        }

        dbContext.Vehicles.AddRange(allVehicles);
        dbContext.Drivers.AddRange(allDrivers);
        dbContext.SaveChanges();

        Console.WriteLine($"- Всего машин: {allVehicles.Count}");
        Console.WriteLine($"- Всего водителей: {allDrivers.Count}");
        
        // Устанавливаем связи между автомобилями и водителями
        // Примерно каждая 10-я машина с активным водителем (0.1)
        // От 1 до 5 водителей на машину (или меньше, если водителей меньше)
        int maxDriversPerVehicle = Math.Min(10, driversPerEnterprise);
        generator.EstablishVehicleDriverRelationships(allVehicles, allDrivers, 0.1, 0.7, 0, maxDriversPerVehicle);
        dbContext.SaveChanges();

        transaction.Commit();

        PrintResults(allVehicles, allDrivers);
        
        if (!string.IsNullOrEmpty(outputPath))
        {
            SaveToFile(allVehicles, allDrivers, outputPath);
        }
    }

    static void PrintResults(List<Vehicle> vehicles, List<Driver> drivers)
    {
        // Подробная статистика
        int activeVehiclesCount = vehicles.Count(v => v.ActiveAssignedDriver != null);
        int activeDriversCount = drivers.Count(d => d.ActiveAssignedVehicle != null);
        int vehiclesWithDrivers = vehicles.Count(v => v.AssignedDrivers.Any());
        int driversWithVehicles = drivers.Count(d => d.AssignedVehicles.Any());
        
        Console.WriteLine($"\n=== СТАТИСТИКА ===");
        Console.WriteLine($"  Всего автомобилей: {vehicles.Count}");
        Console.WriteLine($"  Всего водителей: {drivers.Count}");
        Console.WriteLine();
        Console.WriteLine($"  Автомобилей с назначенными водителями: {vehiclesWithDrivers}");
        Console.WriteLine($"  Водителей с назначенными автомобилями: {driversWithVehicles}");
        Console.WriteLine($"  Автомобилей с активными водителями: {activeVehiclesCount}");
        Console.WriteLine($"  Водителей с активными автомобилями: {activeDriversCount}");
        Console.WriteLine();
        Console.WriteLine($"  Процент автомобилей с водителями: {(double)vehiclesWithDrivers / vehicles.Count * 100:F1}%");
        Console.WriteLine($"  Процент водителей с автомобилями: {(double)driversWithVehicles / drivers.Count * 100:F1}%");
        Console.WriteLine($"  Процент активных автомобилей: {(double)activeVehiclesCount / vehicles.Count * 100:F1}%");
        Console.WriteLine($"  Процент активных водителей: {(double)activeDriversCount / drivers.Count * 100:F1}%");
    }

    static void SaveToFile(List<Vehicle> vehicles, List<Driver> drivers, string filePath)
    {
        try
        {
            SerializableData serializableData = new SerializableData
            {
                Vehicles = vehicles.Select(v => new VehicleDto
                {
                    Id = v.Id,
                    ModelId = v.ModelId,
                    EnterpriseId = v.EnterpriseId,
                    VinNumber = v.VinNumber,
                    Price = v.Price,
                    ManufactureYear = v.ManufactureYear,
                    Mileage = v.Mileage,
                    Color = v.Color,
                    ActiveDriverId = v.ActiveAssignedDriver?.Id,
                    AssignedDriverIds = v.AssignedDrivers.Select(d => d.Id).ToList()
                }).ToList(),
                
                Drivers = drivers.Select(d => new DriverDto
                {
                    Id = d.Id,
                    EnterpriseId = d.EnterpriseId,
                    FullName = d.FullName,
                    DriverLicenseNumber = d.DriverLicenseNumber,
                    ActiveVehicleId = d.ActiveAssignedVehicle?.Id,
                    AssignedVehicleIds = d.AssignedVehicles.Select(v => v.Id).ToList()
                }).ToList()
            };

            string json = JsonSerializer.Serialize(serializableData, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            
            File.WriteAllText(filePath, json);
            Console.WriteLine($"\nДанные сохранены в файл: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
        }
    }
}