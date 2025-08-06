using System.CommandLine;
using CarPark.Models.Drivers;
using CarPark.Models.Enterprises;
using CarPark.Models.Models;
using CarPark.Models.Vehicles;
using CarPark.Data;
using Microsoft.EntityFrameworkCore;

namespace CarPark.DataGenerator;

internal class Program
{
    static int Main(string[] args)
    {
        var rootCommand = new RootCommand("Утилита генерации тестовых данных для CarPark");

        // Команда для генерации в файл
        var enterprisesFileOption = new Option<int>("--enterprises", "-e") { Description = "Количество предприятий", Required = true };
        var vehiclesPerEnterpriseFileOption = new Option<int>("--vehicles-per-enterprise", "-v") { Description = "Количество машин на предприятие", Required = true };
        var outputOption = new Option<string>("--output", "-o") { Description = "Путь к выходному файлу", Required = true };

        var generateFileCommand = new Command("generate-file", "Генерировать данные в файл")
        {
            enterprisesFileOption,
            vehiclesPerEnterpriseFileOption,
            outputOption
        };

        generateFileCommand.SetAction(parseResult => GenerateToFile(
            parseResult.GetValue(enterprisesFileOption),
            parseResult.GetValue(vehiclesPerEnterpriseFileOption),
            parseResult.GetValue(outputOption)));

        // Команда для генерации в БД
        var enterprisesDbOption = new Option<int>("--enterprises", "-e") { Description = "Количество предприятий", Required = true };
        var vehiclesPerEnterpriseDbOption = new Option<int>("--vehicles-per-enterprise", "-v") { Description = "Количество машин на предприятие", Required = true };
        var connectionStringOption = new Option<string>("--connection-string", "-c") { Description = "Строка подключения к БД", Required = true };

        var generateDbCommand = new Command("generate-db", "Генерировать данные в базу данных")
        {
            enterprisesDbOption,
            vehiclesPerEnterpriseDbOption,
            connectionStringOption
        };

        generateDbCommand.SetAction(parseResult => GenerateToDatabase(
            parseResult.GetValue(enterprisesDbOption),
            parseResult.GetValue(vehiclesPerEnterpriseDbOption),
            parseResult.GetValue(connectionStringOption)));

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

        var generator = new DataGenerator();
        var data = generator.GenerateAll(enterprisesCount, vehiclesPerEnterprise);

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

        var generator = new DataGenerator();
        var data = generator.GenerateAll(enterprisesCount, vehiclesPerEnterprise);

        PrintResults(data);
        InsertToDatabase(data, connectionString);
    }

    static void PrintResults(GeneratedData data)
    {
        Console.WriteLine("=== СГЕНЕРИРОВАННЫЕ ДАННЫЕ ===");
        
        Console.WriteLine($"\nПредприятия ({data.Enterprises.Count}):");
        foreach (var enterprise in data.Enterprises)
        {
            Console.WriteLine($"  Название: {enterprise.Name}, Адрес: {enterprise.LegalAddress}");
        }

        Console.WriteLine($"\nМодели ({data.Models.Count}):");
        foreach (var model in data.Models)
        {
            Console.WriteLine($"  Модель: {model.ModelName}, Тип: {model.VehicleType}");
        }

        Console.WriteLine($"\nМашины ({data.Vehicles.Count}):");
        foreach (var vehicle in data.Vehicles)
        {
            var activeDriver = vehicle.ActiveAssignedDriver != null ? $" (Водитель: {vehicle.ActiveAssignedDriver.FullName})" : "";
            Console.WriteLine($"  VIN: {vehicle.VinNumber}, Цвет: {vehicle.Color}, Цена: {vehicle.Price:C}{activeDriver}");
        }

        Console.WriteLine($"\nВодители ({data.Drivers.Count}):");
        foreach (var driver in data.Drivers)
        {
            var activeVehicle = driver.ActiveAssignedVehicle != null ? $" (Машина: {driver.ActiveAssignedVehicle.VinNumber})" : "";
            Console.WriteLine($"  ФИО: {driver.FullName}, Права: {driver.DriverLicenseNumber}{activeVehicle}");
        }

        var activeVehicles = data.Vehicles.Count(v => v.ActiveAssignedDriver != null);
        var activeDrivers = data.Drivers.Count(d => d.ActiveAssignedVehicle != null);
        
        Console.WriteLine($"\nСтатистика:");
        Console.WriteLine($"  Активных машин с водителями: {activeVehicles}");
        Console.WriteLine($"  Активных водителей с машинами: {activeDrivers}");
        Console.WriteLine($"  Процент активных машин: {(double)activeVehicles / data.Vehicles.Count * 100:F1}%");
    }

    static void SaveToFile(GeneratedData data, string filePath)
    {
        try
        {
            var serializableData = new SerializableData
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

            var json = System.Text.Json.JsonSerializer.Serialize(serializableData, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
            
            File.WriteAllText(filePath, json);
            Console.WriteLine($"\nДанные сохранены в файл: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
        }
    }

    static void InsertToDatabase(GeneratedData data, string connectionString)
    {
        try
        {
            Console.WriteLine("Подключение к базе данных...");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(connectionString)
                         .UseSnakeCaseNamingConvention();

            using var context = new ApplicationDbContext(optionsBuilder.Options);

            Console.WriteLine("Вставка данных в базу данных...");

            // Вставляем модели
            Console.WriteLine($"Вставка {data.Models.Count} моделей...");
            var modelsToInsert = data.Models.Select(m => new Model
            {
                Id = default,
                ModelName = m.ModelName,
                VehicleType = m.VehicleType,
                SeatsCount = m.SeatsCount,
                MaxLoadingWeightKg = m.MaxLoadingWeightKg,
                EnginePowerKW = m.EnginePowerKW,
                TransmissionType = m.TransmissionType,
                FuelSystemType = m.FuelSystemType,
                FuelTankVolumeLiters = m.FuelTankVolumeLiters
            }).ToList();

            context.Models.AddRange(modelsToInsert);
            context.SaveChanges();

            // Вставляем предприятия
            Console.WriteLine($"Вставка {data.Enterprises.Count} предприятий...");
            var enterprisesToInsert = data.Enterprises.Select(e => new Enterprise
            {
                Id = default,
                Name = e.Name,
                LegalAddress = e.LegalAddress,
                Managers = new List<CarPark.Models.Managers.Manager>()
            }).ToList();

            context.Enterprises.AddRange(enterprisesToInsert);
            context.SaveChanges();

            // Вставляем машины
            Console.WriteLine($"Вставка {data.Vehicles.Count} машин...");
            var vehiclesToInsert = new List<Vehicle>();
            
            for (int i = 0; i < data.Vehicles.Count; i++)
            {
                var originalVehicle = data.Vehicles[i];
                var enterpriseIndex = i / data.Vehicles.Count * enterprisesToInsert.Count;
                var modelIndex = i % modelsToInsert.Count;
                
                var vehicle = new Vehicle
                {
                    Id = default,
                    ModelId = modelsToInsert[modelIndex].Id,
                    EnterpriseId = enterprisesToInsert[enterpriseIndex].Id,
                    VinNumber = originalVehicle.VinNumber,
                    Price = originalVehicle.Price,
                    ManufactureYear = originalVehicle.ManufactureYear,
                    Mileage = originalVehicle.Mileage,
                    Color = originalVehicle.Color,
                    AssignedDrivers = new List<Driver>(),
                    ActiveAssignedDriver = null
                };
                
                vehiclesToInsert.Add(vehicle);
            }

            context.Vehicles.AddRange(vehiclesToInsert);
            context.SaveChanges();

            // Вставляем водителей
            Console.WriteLine($"Вставка {data.Drivers.Count} водителей...");
            var driversToInsert = new List<Driver>();
            
            for (int i = 0; i < data.Drivers.Count; i++)
            {
                var originalDriver = data.Drivers[i];
                var enterpriseIndex = i % enterprisesToInsert.Count;
                
                var driver = new Driver
                {
                    Id = default,
                    EnterpriseId = enterprisesToInsert[enterpriseIndex].Id,
                    FullName = originalDriver.FullName,
                    DriverLicenseNumber = originalDriver.DriverLicenseNumber,
                    AssignedVehicles = new List<Vehicle>(),
                    ActiveAssignedVehicle = null
                };
                
                driversToInsert.Add(driver);
            }

            context.Drivers.AddRange(driversToInsert);
            context.SaveChanges();

            // Обновляем связи между водителями и машинами
            Console.WriteLine("Обновление связей между водителями и машинами...");
            var dbVehicles = context.Vehicles.ToList();
            var dbDrivers = context.Drivers.ToList();
            
            for (int i = 0; i < data.Vehicles.Count; i++)
            {
                var originalVehicle = data.Vehicles[i];
                var dbVehicle = dbVehicles[i];
                
                if (originalVehicle.ActiveAssignedDriver != null)
                {
                    var driverIndex = data.Drivers.IndexOf(originalVehicle.ActiveAssignedDriver);
                    if (driverIndex >= 0 && driverIndex < dbDrivers.Count)
                    {
                        var dbDriver = dbDrivers[driverIndex];
                        dbVehicle.ActiveAssignedDriver = dbDriver;
                        dbDriver.ActiveAssignedVehicle = dbVehicle;
                    }
                }
            }
            context.SaveChanges();

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