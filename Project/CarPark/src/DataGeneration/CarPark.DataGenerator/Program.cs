using CarPark.Data;
using CarPark.Models;
using CarPark.TimeZones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.CommandLine;
using System.Text.Json;
using CarPark.Drivers;
using CarPark.Enterprises;
using CarPark.Vehicles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Security.Cryptography;
using CarPark.TimeZones.Providers;
using CarPark.Vehicles.Services;
using CarPark.Managers.Services;

namespace CarPark.DataGenerator;

public sealed class Program
{
    public static async Task<int> Main(string[] args)
    {
        Icu.Wrapper.Init();

        RootCommand rootCommand = new RootCommand("Утилита генерации тестовых данных для CarPark");

        Command generateCommand = new Command("generate", "Генерация данных");
        
        generateCommand.Add(CreateGenerateVehiclesAndDriversCommand());
        generateCommand.Add(CreateNewFullDemoCommandCommand());
        generateCommand.Add(CreateNewSeedReferenceCommand());

        rootCommand.Add(generateCommand);

        Icu.Wrapper.Cleanup();

        return await rootCommand.Parse(args).InvokeAsync();
    }

    static Command CreateNewFullDemoCommandCommand()
    {
        Option<int> seedOption = new Option<int>("--seed") { Required = true, Description = "Seed для генератора" };
        Option<int> enterprisesOption = new Option<int>("--enterprises", "-e") { Required = true };
        Option<int> vehiclesOption = new Option<int>("--vehicles-per-enterprise", "-v") { Required = true };
        Option<int> driversOption = new Option<int>("--drivers-per-enterprise", "-d") { Required = true };
        Option<string> exportVehicleIdsOption = new Option<string>("--export-vehicle-ids", "-o") { Description = "Путь к файлу для экспорта ID активных автомобилей" };
        Option<string> connStringFullOption = new Option<string>("--connection-string", "-c") { Required = true };

        Command fullDemoCommand = new Command("full-demo", "Генерация полного набора демо-данных")
        {
            seedOption,
            enterprisesOption,
            vehiclesOption,
            driversOption,
            exportVehicleIdsOption,
            connStringFullOption
        };

        fullDemoCommand.SetAction(parseResult => GenerateFullDemo(
            parseResult.GetRequiredValue<int>(seedOption),
            parseResult.GetRequiredValue<int>(enterprisesOption),
            parseResult.GetRequiredValue<int>(vehiclesOption),
            parseResult.GetRequiredValue<int>(driversOption),
            parseResult.GetValue<string>(exportVehicleIdsOption),
            parseResult.GetRequiredValue<string>(connStringFullOption)
            ));

        return fullDemoCommand;
    }

    static Command CreateNewSeedReferenceCommand()
    {
        Option<string> connStringSeedOption = new Option<string>("--connection-string", "-c") { Description = "Строка подключения к БД", Required = true };

        Option<int> seedOption = new Option<int>("--seed") { Required = true, Description = "Seed для генератора" };

        Command seedReferenceCommand = new Command("seed-reference", "Генерация справочных данных")
        {
            seedOption,
            connStringSeedOption
        };

        seedReferenceCommand.SetAction(parseResult => GenerateSeedReference(
            parseResult.GetValue(seedOption),
            parseResult.GetRequiredValue(connStringSeedOption)));

        return seedReferenceCommand;
    }

    static async Task GenerateSeedReference(int seed, string connectionString)
    {
        Console.WriteLine($"Генерация справочных данных с seed = {seed}");

        IServiceProvider serviceProvider = CreateServiceProvider(connectionString);
        await using ApplicationDbContext context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // 1. Временные зоны (TzInfo)
        using var tzService = new LocalIcuTimezoneService();
        var tzInfos = GenerateTzInfos(tzService);

        foreach (var info in tzInfos.GroupBy(x => x.Id).Where(x => x.Count() > 1).SelectMany(x => x))
        {
            Console.WriteLine($"{info.Id} {info.IanaTzId} {info.WindowsTzId}");
        }

        context.TzInfos.AddRange(tzInfos);
        await context.SaveChangesAsync();
        Console.WriteLine($"Создано {tzInfos.Count} временных зон");
    }

    static Command CreateGenerateVehiclesAndDriversCommand()
    {
        Option<string> enterpriseIdsOption = new Option<string>("--enterprise-ids", "-e") { Description = "Список ID предприятий через запятую", Required = true };
        Option<int> vehiclesPerEnterpriseOption = new Option<int>("--vehicles-per-enterprise", "-v") { Description = "Количество машин на предприятие", Required = true };
        Option<int> driversPerEnterpriseOption = new Option<int>("--drivers-per-enterprise", "-d") { Description = "Количество водителей на предприятие", Required = true };
        Option<string> outputVehiclesOption = new Option<string>("--output", "-o") { Description = "Путь к выходному JSON файлу" };
        Option<string> connectionStringVehiclesOption = new Option<string>("--connection-string", "-c") { Description = "Строка подключения к БД", Required = true };
        Option<int> seedOption = new Option<int>("--seed") { Description = "Seed для генератора", Required = true };

        Command vehiclesDriversCommand = new Command("old-vehicles-drivers", "Генерация автомобилей и водителей")
        {
            enterpriseIdsOption,
            vehiclesPerEnterpriseOption,
            driversPerEnterpriseOption,
            outputVehiclesOption,
            connectionStringVehiclesOption,
            seedOption
        };

        vehiclesDriversCommand.SetAction(async parseResult => await GenerateVehiclesAndDrivers(
            parseResult.GetRequiredValue(enterpriseIdsOption),
            parseResult.GetValue(vehiclesPerEnterpriseOption),
            parseResult.GetValue(driversPerEnterpriseOption),
            parseResult.GetValue(outputVehiclesOption),
            parseResult.GetValue(connectionStringVehiclesOption)!,
            parseResult.GetValue(seedOption)));

        return vehiclesDriversCommand;
    }

    static IServiceProvider CreateServiceProvider(string connectionString)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = connectionString
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);

        var configurator = new InfrastractureModuleConfigurator();
        configurator.ConfigureModule(services, configuration, null);

        // Register domain services
        services.AddScoped<IVehiclesService, VehiclesService>();
        services.AddScoped<IManagersService, ManagersService>();
        services.AddScoped<CarPark.Enterprises.Services.IEnterprisesService, CarPark.Enterprises.Services.EnterprisesService>();
        services.AddScoped<CarPark.Models.Services.IModelsService, CarPark.Models.Services.ModelsService>();
        services.AddScoped<CarPark.Drivers.Services.IDriversService, CarPark.Drivers.Services.DriversService>();

        return services.BuildServiceProvider();
    }

    static List<Model> LoadModelsFromDatabase(ApplicationDbContext context)
    {
        List<Model> models = context.Models.ToList();
        Console.WriteLine($"Загружено {models.Count} моделей из базы данных");
        return models;
    }

    static async Task<List<Enterprise>> FindEnterprisesAsync(List<Guid> enterpriseIds, ApplicationDbContext context)
    {
        List<Enterprise> existingEnterprises = await context.Enterprises
            .Where(e => enterpriseIds.Contains(e.Id))
            .ToListAsync();

        List<Enterprise> missingEnterprises = existingEnterprises.ExceptBy(enterpriseIds, e => e.Id).ToList();
        if (missingEnterprises.Any())
        {
            Console.WriteLine($"Предупреждение: Следующие предприятия не найдены в БД: {string.Join(", ", missingEnterprises.Select(e => e.Id))}");
        }

        Console.WriteLine($"Найдено {existingEnterprises.Count} существующих предприятий из {enterpriseIds.Count} запрошенных");
        return existingEnterprises;
    }

    static async Task GenerateVehiclesAndDrivers(string enterpriseIdsString, int vehiclesPerEnterprise, int driversPerEnterprise, string? outputPath, string connectionString, int seed)
    {
        // Парсим список ID предприятий
        List<Guid> enterpriseIds = enterpriseIdsString.Split(',')
            .Select(id => Guid.Parse(id.Trim()))
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
        IServiceProvider serviceProvider = CreateServiceProvider(connectionString);
        await using ApplicationDbContext dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

        await using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync();

        // Загружаем модели из БД
        List<Model> models = LoadModelsFromDatabase(dbContext);
        if (!models.Any())
        {
            Console.WriteLine("Ошибка: Не удалось загрузить модели из базы данных");
            return;
        }

        // Проверяем существование предприятий в БД
        List<Enterprise> existingEnterprises = await FindEnterprisesAsync(enterpriseIds, dbContext);
        if (!existingEnterprises.Any())
        {
            Console.WriteLine("Ошибка: Не найдено ни одного существующего предприятия");
            return;
        }

        DataGenerator generator = new DataGenerator(seed, serviceProvider.GetRequiredService<IVehiclesService>(), serviceProvider.GetRequiredService<CarPark.Drivers.Services.IDriversService>());
        
        // Генерируем автомобили и водителей для каждого предприятия
        List<Vehicle> allVehicles = new List<Vehicle>();
        List<Driver> allDrivers = new List<Driver>();
        
        foreach (Enterprise enterprise in existingEnterprises)
        {
            // Генерируем автомобили для предприятия
            List<Vehicle> enterpriseVehicles = generator.GenerateVehicles(enterprise, models)
                .Take(vehiclesPerEnterprise)
                .ToList();

            // Генерируем водителей для предприятия
            List<Driver> enterpriseDrivers = generator.GenerateDrivers(enterprise)
                .Take(driversPerEnterprise)
                .ToList();
            
            allVehicles.AddRange(enterpriseVehicles);
            allDrivers.AddRange(enterpriseDrivers);
        }

        dbContext.Vehicles.AddRange(allVehicles);
        dbContext.Drivers.AddRange(allDrivers);
        await dbContext.SaveChangesAsync();

        Console.WriteLine($"- Всего машин: {allVehicles.Count}");
        Console.WriteLine($"- Всего водителей: {allDrivers.Count}");
        
        // Устанавливаем связи между автомобилями и водителями
        // Примерно каждая 10-я машина с активным водителем (0.1)
        // От 1 до 5 водителей на машину (или меньше, если водителей меньше)
        int maxDriversPerVehicle = Math.Min(10, driversPerEnterprise);
        generator.EstablishVehicleDriverRelationships(allVehicles, allDrivers, 0.1, 0.7, 0, maxDriversPerVehicle);
        await dbContext.SaveChangesAsync();

        await transaction.CommitAsync();

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
                    ModelId = v.Model.Id,
                    EnterpriseId = v.Enterprise.Id,
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
            throw;
        }
    }


    static async Task GenerateFullDemo(
        int seed,
        int enterprisesCount,
        int vehiclesPerEnterprise,
        int driversPerEnterprise,
        string? exportVehicleIdsFile,
        string connectionString)
    {
        Console.WriteLine($"   Генерация полного набора демо-данных с seed={seed}");
        Console.WriteLine($"   Предприятий: {enterprisesCount}");
        Console.WriteLine($"   Машин на предприятие: {vehiclesPerEnterprise}");
        Console.WriteLine($"   Водителей на предприятие: {driversPerEnterprise}");

        IServiceProvider serviceProvider = CreateServiceProvider(connectionString);
        await using ApplicationDbContext context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // 1. Enterprises
            EnterprisesGenerator enterprisesGen = new EnterprisesGenerator(
                serviceProvider.GetRequiredService<CarPark.Enterprises.Services.IEnterprisesService>(), seed);
            List<Enterprise> enterprises = enterprisesGen.GenerateEnterprises(enterprisesCount);
            context.Enterprises.AddRange(enterprises);
            await context.SaveChangesAsync();
            Console.WriteLine($"Создано {enterprises.Count} предприятий");

            // 2. Managers
            var managersGen = new ManagersGenerator(serviceProvider.GetRequiredService<IManagersService>(), seed);
            var (identityUsers, managers) = managersGen.GenerateManagers(enterprises);
            context.Users.AddRange(identityUsers);
            await context.SaveChangesAsync();
            context.Managers.AddRange(managers);
            await context.SaveChangesAsync();
            Console.WriteLine($"Создано {managers.Count} менеджеров");

            // 2.5. Models (if not exist)
            if (!context.Models.Any())
            {
                var modelsGen =
                    new ModelsGenerator(serviceProvider.GetRequiredService<CarPark.Models.Services.IModelsService>(),
                        seed);
                var modelsList = modelsGen.GenerateModels();
                context.Models.AddRange(modelsList);
                await context.SaveChangesAsync();
                Console.WriteLine($"Создано {modelsList.Count} моделей автомобилей");
            }

            // 3. Vehicles and Drivers
            DataGenerator dataGen = new DataGenerator(seed, serviceProvider.GetRequiredService<IVehiclesService>(),
                serviceProvider.GetRequiredService<CarPark.Drivers.Services.IDriversService>());
            List<Model> models = context.Models.ToList();

            List<Vehicle> allVehicles = new List<Vehicle>();
            List<Driver> allDrivers = new List<Driver>();

            foreach (Enterprise enterprise in enterprises)
            {
                List<Vehicle> vehicles = dataGen.GenerateVehicles(enterprise, models)
                    .Take(vehiclesPerEnterprise)
                    .ToList();

                List<Driver> drivers = dataGen.GenerateDrivers(enterprise)
                    .Take(driversPerEnterprise)
                    .ToList();

                allVehicles.AddRange(vehicles);
                allDrivers.AddRange(drivers);
            }

            context.Vehicles.AddRange(allVehicles);
            context.Drivers.AddRange(allDrivers);
            await context.SaveChangesAsync();
            Console.WriteLine($"Создано {allVehicles.Count} автомобилей");
            Console.WriteLine($"Создано {allDrivers.Count} водителей");

            // 4. Vehicle-Driver relationships
            dataGen.EstablishVehicleDriverRelationships(allVehicles, allDrivers,
                activeDriverRatio: 0.3,
                assignmentRatio: 0.7,
                minDriversPerVehicle: 0,
                maxDriversPerVehicle: Math.Min(10, driversPerEnterprise));
            await context.SaveChangesAsync();
            Console.WriteLine($"Установлены связи Vehicle-Driver");

            // 5. Export vehicle IDs если нужно
            if (!string.IsNullOrEmpty(exportVehicleIdsFile))
            {
                List<string> activeVehicles = allVehicles
                    .Where(v => v.ActiveAssignedDriver != null)
                    .Select(v => v.Id.ToString())
                    .ToList();

                await File.WriteAllLinesAsync(exportVehicleIdsFile, activeVehicles);
                Console.WriteLine(
                    $"Экспортировано {activeVehicles.Count} активных vehicle IDs в {exportVehicleIdsFile}");
            }

            await transaction.CommitAsync();
            Console.WriteLine($"Генерация успешно завершена!");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Ошибка: {ex.ToString()}");
            throw;
        }
    }

    static List<TzInfo> GenerateTzInfos(LocalIcuTimezoneService tzService)
    {
        IReadOnlyCollection<string> ianaIds = tzService.GetAvailableIanaIds();
        IReadOnlyCollection<KeyValuePair<string, string>> mappings = tzService.MapIanaIdsToWindowsIds(ianaIds);
        List<TzInfo> result = new List<TzInfo>();

        foreach (var mapping in mappings)
        {
            Guid id = GenerateDeterministicGuid(mapping.Key + mapping.Value);
            result.Add(new TzInfo(id, mapping.Key, mapping.Value));
        }

        return result;
    }

    private static Guid GenerateDeterministicGuid(string input)
    {
        // Choose a namespace — this can be any GUID you define for your app
        var namespaceGuid = Guid.Parse("6ba7b811-9dad-11d1-80b4-00c04fd430c8"); // DNS namespace, per RFC 4122

        // Convert namespace + name to bytes
        byte[] namespaceBytes = namespaceGuid.ToByteArray();
        SwapByteOrder(namespaceBytes);
        byte[] nameBytes = Encoding.UTF8.GetBytes(input);

        // Compute hash of namespace + name
        byte[] hash = SHA1.HashData(Combine(namespaceBytes, nameBytes));

        // Use first 16 bytes of the hash to make a GUID
        byte[] newGuid = new byte[16];
        Array.Copy(hash, 0, newGuid, 0, 16);

        // Set version (5) and variant bits (RFC 4122)
        newGuid[6] = (byte)((newGuid[6] & 0x0F) | (5 << 4));
        newGuid[8] = (byte)((newGuid[8] & 0x3F) | 0x80);

        SwapByteOrder(newGuid);
        return new Guid(newGuid);
    }

    private static byte[] Combine(byte[] a, byte[] b)
    {
        byte[] result = new byte[a.Length + b.Length];
        Buffer.BlockCopy(a, 0, result, 0, a.Length);
        Buffer.BlockCopy(b, 0, result, a.Length, b.Length);
        return result;
    }

    // Swap to match network byte order (RFC 4122)
    private static void SwapByteOrder(byte[] guid)
    {
        void Swap(int a, int b) { byte t = guid[a]; guid[a] = guid[b]; guid[b] = t; }
        Swap(0, 3); Swap(1, 2); Swap(4, 5); Swap(6, 7);
    }
}