using CarPark.Data;
using CarPark.Models.Drivers;
using CarPark.Models.Enterprises;
using CarPark.Models.Managers;
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

        // Команда для генерации в файл
        Option<int> enterprisesFileOption = new Option<int>("--enterprises", "-e") { Description = "Количество предприятий", Required = true };
        Option<int> vehiclesPerEnterpriseFileOption = new Option<int>("--vehicles-per-enterprise", "-v") { Description = "Количество машин на предприятие", Required = true };
        Option<string> outputOption = new Option<string>("--output", "-o") { Description = "Путь к выходному файлу", Required = true };

        Command generateFileCommand = new Command("generate-file", "Генерировать данные в файл")
        {
            enterprisesFileOption,
            vehiclesPerEnterpriseFileOption,
            outputOption
        };

        generateFileCommand.SetAction(parseResult => GenerateToFile(
            parseResult.GetValue(enterprisesFileOption),
            parseResult.GetValue(vehiclesPerEnterpriseFileOption),
            parseResult.GetValue(outputOption)!));

        // Команда для генерации в БД
        Option<int> enterprisesDbOption = new Option<int>("--enterprises", "-e") { Description = "Количество предприятий", Required = true };
        Option<int> vehiclesPerEnterpriseDbOption = new Option<int>("--vehicles-per-enterprise", "-v") { Description = "Количество машин на предприятие", Required = true };
        Option<string> connectionStringOption = new Option<string>("--connection-string", "-c") { Description = "Строка подключения к БД", Required = true };

        Command generateDbCommand = new Command("generate-db", "Генерировать данные в базу данных")
        {
            enterprisesDbOption,
            vehiclesPerEnterpriseDbOption,
            connectionStringOption
        };

        generateDbCommand.SetAction(parseResult => GenerateToDatabase(
            parseResult.GetValue(enterprisesDbOption),
            parseResult.GetValue(vehiclesPerEnterpriseDbOption),
            parseResult.GetValue(connectionStringOption)!));

        rootCommand.Add(generateFileCommand);
        rootCommand.Add(generateDbCommand);

        return rootCommand.Parse(args).Invoke();
    }

    static void GenerateToFile(int enterprisesCount, int vehiclesPerEnterprise, string outputPath)
    {
        Console.WriteLine($"Генерация данных в файл:");
        Console.WriteLine($"- Предприятий: {enterprisesCount}");
        Console.WriteLine($"- Машин на предприятие: {vehiclesPerEnterprise}");
        Console.WriteLine($"- Всего машин: {enterprisesCount * vehiclesPerEnterprise}");
        Console.WriteLine($"- Выходной файл: {outputPath}");
        Console.WriteLine();

        DataGenerator generator = new DataGenerator();
        GeneratedData data = generator.GenerateAll(enterprisesCount, 20, vehiclesPerEnterprise);

        PrintResults(data);
        SaveToFile(data, outputPath);
    }

    static void GenerateToDatabase(int enterprisesCount, int vehiclesPerEnterprise, string connectionString)
    {
        Console.WriteLine($"Генерация данных в базу данных:");
        Console.WriteLine($"- Предприятий: {enterprisesCount}");
        Console.WriteLine($"- Машин на предприятие: {vehiclesPerEnterprise}");
        Console.WriteLine($"- Всего машин: {enterprisesCount * vehiclesPerEnterprise}");
        Console.WriteLine();

        DataGenerator generator = new DataGenerator();
        GeneratedData data = generator.GenerateAll(enterprisesCount, 20, vehiclesPerEnterprise);

        PrintResults(data);
        InsertToDatabaseV2(data, connectionString);
        //InsertToDatabase(data, connectionString);
    }

    static void PrintResults(GeneratedData data)
    {
        Console.WriteLine("=== СГЕНЕРИРОВАННЫЕ ДАННЫЕ ===");
        
        Console.WriteLine($"\nПредприятия ({data.Enterprises.Count}):");
        foreach (Enterprise enterprise in data.Enterprises)
        {
            Console.WriteLine($"  Название: {enterprise.Name}, Адрес: {enterprise.LegalAddress}");
        }

        Console.WriteLine($"\nМодели ({data.Models.Count}):");
        foreach (Model model in data.Models)
        {
            Console.WriteLine($"  Модель: {model.ModelName}, Тип: {model.VehicleType}");
        }

        Console.WriteLine($"\nМашины ({data.Vehicles.Count}):");
        foreach (Vehicle vehicle in data.Vehicles)
        {
            string activeDriver = vehicle.ActiveAssignedDriver != null 
                ? $" (Водитель: {vehicle.ActiveAssignedDriver.FullName})" 
                : "";

            Console.WriteLine($"  VIN: {vehicle.VinNumber}, Цвет: {vehicle.Color}, Цена: {vehicle.Price:C}{activeDriver}");
        }

        Console.WriteLine($"\nВодители ({data.Drivers.Count}):");
        foreach (Driver driver in data.Drivers)
        {
            string activeVehicle = driver.ActiveAssignedVehicle != null 
                ? $" (Машина: {driver.ActiveAssignedVehicle.VinNumber})" 
                : "";

            Console.WriteLine($"  ФИО: {driver.FullName}, Права: {driver.DriverLicenseNumber}{activeVehicle}");
        }

        int activeVehiclesCount = data.Vehicles.Count(v => v.ActiveAssignedDriver != null);
        var activeDriversCount = data.Drivers.Count(d => d.ActiveAssignedVehicle != null);
        
        Console.WriteLine($"\nСтатистика:");
        Console.WriteLine($"  Активных машин с водителями: {activeVehiclesCount}");
        Console.WriteLine($"  Активных водителей с машинами: {activeDriversCount}");
        Console.WriteLine($"  Процент активных машин: {(double)activeVehiclesCount / data.Vehicles.Count * 100:F1}%");
    }

    static void SaveToFile(GeneratedData data, string filePath)
    {
        try
        {
            SerializableData serializableData = new SerializableData
            {
                Enterprises = data.Enterprises.Select(e => new EnterpriseDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    LegalAddress = e.LegalAddress
                }).ToList(),
                
                Models = data.Models.Select(m => new ModelDto
                {
                    Id = m.Id,
                    ModelName = m.ModelName,
                    VehicleType = m.VehicleType,
                    SeatsCount = m.SeatsCount,
                    MaxLoadingWeightKg = m.MaxLoadingWeightKg,
                    EnginePowerKW = m.EnginePowerKW,
                    TransmissionType = m.TransmissionType,
                    FuelSystemType = m.FuelSystemType,
                    FuelTankVolumeLiters = m.FuelTankVolumeLiters
                }).ToList(),
                
                Vehicles = data.Vehicles.Select(v => new VehicleDto
                {
                    Id = v.Id,
                    ModelId = v.ModelId,
                    EnterpriseId = v.EnterpriseId,
                    VinNumber = v.VinNumber,
                    Price = v.Price,
                    ManufactureYear = v.ManufactureYear,
                    Mileage = v.Mileage,
                    Color = v.Color,
                    ActiveDriverId = v.ActiveAssignedDriver?.Id
                }).ToList(),
                
                Drivers = data.Drivers.Select(d => new DriverDto
                {
                    Id = d.Id,
                    EnterpriseId = d.EnterpriseId,
                    FullName = d.FullName,
                    DriverLicenseNumber = d.DriverLicenseNumber,
                    ActiveVehicleId = d.ActiveAssignedVehicle?.Id
                }).ToList()
            };

            string json = JsonSerializer.Serialize(serializableData, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
            
            File.WriteAllText(filePath, json);
            Console.WriteLine($"\nДанные сохранены в файл: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
        }
    }

    static void InsertToDatabaseV2(GeneratedData data, string connectionString)
    {
        try
        {
            Console.WriteLine("Подключение к базе данных...");

            DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(connectionString)
                         .UseSnakeCaseNamingConvention();

            using ApplicationDbContext context = new ApplicationDbContext(optionsBuilder.Options);

            Console.WriteLine("Вставка данных в базу данных...");

            using IDbContextTransaction transaction = context.Database.BeginTransaction();

            // Вставляем модели
            Console.WriteLine($"Вставка {data.Models.Count} моделей...");

            List<Model> newModels = new List<Model>(data.Models.Count);
            Dictionary<int, int> originalToNewModelsIdsMap = new Dictionary<int, int>();

            foreach (Model originalModel in data.Models)
            {
                Model newModel = new Model
                {
                    Id = default,
                    ModelName = originalModel.ModelName,
                    VehicleType = originalModel.VehicleType,
                    SeatsCount = originalModel.SeatsCount,
                    MaxLoadingWeightKg = originalModel.MaxLoadingWeightKg,
                    EnginePowerKW = originalModel.EnginePowerKW,
                    TransmissionType = originalModel.TransmissionType,
                    FuelSystemType = originalModel.FuelSystemType,
                    FuelTankVolumeLiters = originalModel.FuelTankVolumeLiters
                };

                context.Models.Add(newModel);
                context.SaveChanges();

                newModels.Add(newModel);
                originalToNewModelsIdsMap.Add(originalModel.Id, newModel.Id);
            }    

            // Вставляем предприятия
            Console.WriteLine($"Вставка {data.Enterprises.Count} предприятий и связанных сущностей...");

            foreach (Enterprise originalEnterpise in data.Enterprises)
            {
                Enterprise newEnterprise = new Enterprise
                {
                    Id = default,
                    Name = originalEnterpise.Name,
                    LegalAddress = originalEnterpise.LegalAddress,
                    Managers = new List<Manager>()
                };

                context.Enterprises.Add(newEnterprise);
                context.SaveChanges();

                List<Vehicle> originalVehicles = data.Vehicles
                    .Where(v => v.EnterpriseId == originalEnterpise.Id)
                    .ToList();
                List<Vehicle> newVehicles = new List<Vehicle>(originalVehicles.Count);
                Dictionary<int, int> originalToNewVehiclesIdsMap = new Dictionary<int, int>();

                foreach (Vehicle originalVehicle in originalVehicles)
                {
                    int originalModelId = originalVehicle.ModelId;
                    int newModelId = originalToNewModelsIdsMap[originalModelId];

                    Vehicle newVehicle = new Vehicle
                    {
                        Id = default,
                        ModelId = newModelId,
                        EnterpriseId = newEnterprise.Id,
                        VinNumber = originalVehicle.VinNumber,
                        Price = originalVehicle.Price,
                        ManufactureYear = originalVehicle.ManufactureYear,
                        Mileage = originalVehicle.Mileage,
                        Color = originalVehicle.Color,
                        AssignedDrivers = new List<Driver>(),
                        ActiveAssignedDriver = null
                    };

                    context.Vehicles.Add(newVehicle);
                    context.SaveChanges();

                    newVehicles.Add(newVehicle);
                    originalToNewVehiclesIdsMap.Add(originalVehicle.Id, newVehicle.Id);
                }
                
                List<Driver> originalDrivers = data.Drivers
                    .Where(d => d.EnterpriseId == originalEnterpise.Id)
                    .ToList();
                List<Driver> newDrivers = new List<Driver>(originalDrivers.Count);
                Dictionary<int, int> originalToNewDriverIdsMap = new Dictionary<int, int>();

                foreach (Driver originalDriver in originalDrivers)
                {
                    Driver newDriver = new Driver
                    {
                        Id = default,
                        EnterpriseId = newEnterprise.Id,
                        FullName = originalDriver.FullName,
                        DriverLicenseNumber = originalDriver.DriverLicenseNumber,
                        AssignedVehicles = new List<Vehicle>(),
                        ActiveAssignedVehicle = null
                    };

                    context.Drivers.Add(newDriver);
                    context.SaveChanges();

                    newDrivers.Add(newDriver);
                    originalToNewDriverIdsMap.Add(originalDriver.Id, newDriver.Id);
                }

                foreach (Vehicle originalVehicle in originalVehicles)
                {
                    int newVehicleId = originalToNewVehiclesIdsMap[originalVehicle.Id];
                    Vehicle newVehicle = newVehicles.First(v => v.Id == newVehicleId);

                    if (originalVehicle.AssignedDrivers.Count != 0)
                    {
                        foreach (Driver originalAssignedDriver in originalVehicle.AssignedDrivers)
                        {
                            int newAssignedDriverId = originalToNewDriverIdsMap[originalAssignedDriver.Id];
                            Driver newAssignedDriver = newDrivers.First(d => d.Id == newAssignedDriverId);

                            newVehicle.AssignedDrivers.Add(newAssignedDriver);
                        }
                    }

                    if (originalVehicle.ActiveAssignedDriver != null)
                    {
                        Driver originalActiveAssignedDriver = originalVehicle.ActiveAssignedDriver;

                        int newActiveAssignedDriverId = originalToNewDriverIdsMap[originalActiveAssignedDriver.Id];
                        Driver newActiveAssignedDriver = newDrivers.First(d => d.Id == newActiveAssignedDriverId);

                        newVehicle.ActiveAssignedDriver = newActiveAssignedDriver;
                    }

                    context.SaveChanges();
                }
            }

            transaction.Commit();

            Console.WriteLine("Данные успешно вставлены в базу данных!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при вставке в базу данных: {ex.Message}");
            Console.WriteLine($"Детали: {ex}");
        }
    }
}

public class GeneratedData
{
    public required List<Enterprise> Enterprises { get; set; } = new();
    public required List<Model> Models { get; set; } = new();
    public required List<Vehicle> Vehicles { get; set; } = new();
    public required List<Driver> Drivers { get; set; } = new();
}