using Bogus;
using CarPark.Drivers;
using CarPark.Enterprises;
using CarPark.Models;
using CarPark.Vehicles;
using CarPark.Vehicles.Services;
using FluentResults;

namespace CarPark.DataGenerator;

public class DataGenerator
{
    private readonly int _seed;
    private readonly Random _random;
    private readonly Faker<VehicleDto> _vehicleFaker;
    private readonly Faker<Driver> _driverFaker;
    private readonly IVehiclesService _vehiclesService;
    private readonly List<Vehicle> _generatedVehicles = new();

    private class VehicleDto
    {
        public required Guid Id { get; init; }

        public required Model Model { get; init; }

        public required Enterprise Enterprise { get; init; }

        public required string VinNumber { get; init; }

        public required decimal Price { get; init; }

        public required int ManufactureYear { get; init; }

        public required int Mileage { get; init; }

        public required string Color { get; init; }

        public required List<Driver> AssignedDrivers { get; init; }

        public required Driver? ActiveAssignedDriver { get; init; }

        public required DateTimeOffset AddedToEnterpriseAt { get; init; }
    }

    public DataGenerator(int seed, IVehiclesService vehiclesService)
    {
        _seed = seed;
        _random = new Random(seed);
        _vehiclesService = vehiclesService;

        // Создать Faker с фиксированным Randomizer
        Randomizer.Seed = new Random(seed);

        _vehicleFaker = new Faker<VehicleDto>("ru")
            .UseSeed(seed)  // Добавить UseSeed
            .RuleFor(v => v.Id, f => default)
            .RuleFor(v => v.VinNumber, f => f.Vehicle.Vin())
            .RuleFor(v => v.Price, f => f.Random.Decimal(500000, 5000000))
            .RuleFor(v => v.ManufactureYear, f => f.Random.Int(2010, 2024))
            .RuleFor(v => v.Mileage, f => f.Random.Int(0, 300000))
            .RuleFor(v => v.Color, f => f.PickRandom("Белый", "Черный", "Серебристый", "Красный", "Синий", "Зеленый", "Серый"))
            .RuleFor(v => v.AssignedDrivers, f => new List<Driver>())
            .RuleFor(v => v.ActiveAssignedDriver, f => (Driver?)null);

        _driverFaker = new Faker<Driver>("ru")
            .UseSeed(seed)  // Добавить UseSeed
            .RuleFor(d => d.Id, f => default)
            .RuleFor(d => d.FullName, f => f.Name.FullName())
            .RuleFor(d => d.DriverLicenseNumber, f => f.Random.Replace("## ## ######"))
            .RuleFor(d => d.AssignedVehicles, f => new List<Vehicle>())
            .RuleFor(d => d.ActiveAssignedVehicle, f => (Vehicle?)null);
    }

    /// <summary>
    /// Генерирует коллекцию автомобилей для указанного предприятия
    /// </summary>
    /// <param name="enterprise">Предприятие</param>
    /// <param name="models">Список доступных моделей</param>
    /// <returns>Коллекция сгенерированных автомобилей</returns>
    public IEnumerable<Vehicle> GenerateVehicles(Enterprise enterprise, List<Model> models)
    {
        return _vehicleFaker
            .RuleFor(v => v.Enterprise, f => enterprise)
            .RuleFor(v => v.Model, f => f.PickRandom(models))
            .RuleFor(v => v.Id, f => GenerateDeterministicGuid())
            .GenerateForever()
            .Select(v =>
            {
                CreateVehicleRequest request = new CreateVehicleRequest
                {
                    Id = v.Id,
                    Model = v.Model,
                    Enterprise = v.Enterprise,
                    VinNumber = v.VinNumber,
                    Price = v.Price,
                    ManufactureYear = v.ManufactureYear,
                    Mileage = v.Mileage,
                    Color = v.Color,
                    AssignedDrivers = v.AssignedDrivers,
                    ActiveAssignedDriver = v.ActiveAssignedDriver,
                    AddedToEnterpriseAt = v.AddedToEnterpriseAt
                };

                return _vehiclesService.CreateVehicle(request).Value;
            });
    }

    /// <summary>
    /// Генерирует коллекцию водителей для указанного предприятия
    /// </summary>
    /// <param name="enterprise">Предприятие</param>
    /// <returns>Коллекция сгенерированных водителей</returns>
    public IEnumerable<Driver> GenerateDrivers(Enterprise enterprise)
    {
        return _driverFaker
            .RuleFor(d => d.EnterpriseId, f => enterprise.Id)
            .RuleFor(d => d.Id, f => GenerateDeterministicGuid())
            .GenerateForever();
    }

    /// <summary>
    /// Устанавливает связи между автомобилями и водителями
    /// </summary>
    /// <param name="vehicles">Список автомобилей</param>
    /// <param name="drivers">Список водителей</param>
    /// <param name="activeDriverRatio">Коэффициент автомобилей с активными водителями (0.0 - 1.0)</param>
    /// <param name="assignmentRatio">Коэффициент автомобилей с назначенными водителями (0.0 - 1.0)</param>
    /// <param name="minDriversPerVehicle">Минимальное количество водителей на автомобиль</param>
    /// <param name="maxDriversPerVehicle">Максимальное количество водителей на автомобиль</param>
    public void EstablishVehicleDriverRelationships(
        List<Vehicle> vehicles,
        List<Driver> drivers,
        double activeDriverRatio,
        double assignmentRatio,
        int minDriversPerVehicle,
        int maxDriversPerVehicle)
    {
        if (vehicles == null || drivers == null)
        {
            throw new ArgumentNullException("Списки автомобилей и водителей не могут быть null");
        }

        if (activeDriverRatio < 0.0 || activeDriverRatio > 1.0)
        {
            throw new ArgumentException("Коэффициент активных водителей должен быть от 0.0 до 1.0", nameof(activeDriverRatio));
        }

        if (assignmentRatio < 0.0 || assignmentRatio > 1.0)
        {
            throw new ArgumentException("Коэффициент назначенных водителей должен быть от 0.0 до 1.0", nameof(assignmentRatio));
        }

        if (minDriversPerVehicle < 0)
        {
            throw new ArgumentException("Минимальное количество водителей на автомобиль должно быть не менее 0", nameof(minDriversPerVehicle));
        }

        if (maxDriversPerVehicle < minDriversPerVehicle)
        {
            throw new ArgumentException("Максимальное количество водителей должно быть не менее минимального", nameof(maxDriversPerVehicle));
        }

        // Группируем автомобили и водителей по предприятиям
        Dictionary<Guid, List<Vehicle>> vehiclesByEnterprise = vehicles
            .GroupBy(v => v.Enterprise.Id)
            .ToDictionary(g => g.Key, g => g.ToList());

        Dictionary<Guid, List<Driver>> driversByEnterprise = drivers
            .GroupBy(d => d.EnterpriseId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Обрабатываем каждое предприятие отдельно
        foreach (Guid enterpriseId in vehiclesByEnterprise.Keys.Intersect(driversByEnterprise.Keys))
        {
            List<Vehicle> enterpriseVehicles = vehiclesByEnterprise[enterpriseId];
            List<Driver> enterpriseDrivers = driversByEnterprise[enterpriseId];

            if (!enterpriseDrivers.Any())
            {
                continue; // Пропускаем предприятия без водителей
            }

            // 1. Создаем назначения водителей на автомобили
            foreach (Vehicle vehicle in enterpriseVehicles)
            {
                // Применяем коэффициент назначенных водителей
                if (_random.NextDouble() < assignmentRatio && enterpriseDrivers.Any())
                {
                    // Случайное количество водителей на автомобиль
                    int selectedDriversCount = _random.Next(
                        minDriversPerVehicle,
                        Math.Min(maxDriversPerVehicle + 1, enterpriseDrivers.Count + 1)
                    );
                    List<Driver> selectedDrivers = enterpriseDrivers
                        .OrderBy(x => _random.Next())
                        .Take(selectedDriversCount)
                        .ToList();

                    List<Driver> currentAssigned = vehicle.AssignedDrivers.ToList();
                    foreach (Driver driver in selectedDrivers)
                    {
                        if (!currentAssigned.Contains(driver))
                        {
                            currentAssigned.Add(driver);
                            driver.AssignedVehicles.Add(vehicle);
                        }
                    }

                    // Update vehicle with new assigned drivers
                    UpdateVehicleRequest updateRequest = new UpdateVehicleRequest
                    {
                        Model = vehicle.Model,
                        Enterprise = vehicle.Enterprise,
                        VinNumber = vehicle.VinNumber,
                        Price = vehicle.Price,
                        ManufactureYear = vehicle.ManufactureYear,
                        Mileage = vehicle.Mileage,
                        Color = vehicle.Color,
                        AssignedDrivers = currentAssigned,
                        ActiveAssignedDriver = vehicle.ActiveAssignedDriver,
                        AddedToEnterpriseAt = vehicle.AddedToEnterpriseAt
                    };

                    _vehiclesService.UpdateVehicle(vehicle, updateRequest);
                }
            }

            // 2. Назначаем активных водителей
            List<Vehicle> vehiclesWithDrivers = enterpriseVehicles
                .Where(v => v.AssignedDrivers.Any())
                .OrderBy(x => _random.Next())
                .ToList();

            HashSet<Guid> usedDriverIds = new HashSet<Guid>();

            double fixedActiveDriverRation =
                (double)enterpriseVehicles.Count / vehiclesWithDrivers.Count * activeDriverRatio;

            foreach (Vehicle vehicle in vehiclesWithDrivers)
            {
                // Применяем коэффициент активных водителей
                if (_random.NextDouble() < fixedActiveDriverRation)
                {
                    Driver? availableDriver = vehicle.AssignedDrivers
                        .FirstOrDefault(d => !usedDriverIds.Contains(d.Id));

                    if (availableDriver != null)
                    {
                        // Update vehicle with active driver
                        UpdateVehicleRequest updateRequest = new UpdateVehicleRequest
                        {
                            Model = vehicle.Model,
                            Enterprise = vehicle.Enterprise,
                            VinNumber = vehicle.VinNumber,
                            Price = vehicle.Price,
                            ManufactureYear = vehicle.ManufactureYear,
                            Mileage = vehicle.Mileage,
                            Color = vehicle.Color,
                            AssignedDrivers = vehicle.AssignedDrivers.ToList(),
                            ActiveAssignedDriver = availableDriver,
                            AddedToEnterpriseAt = vehicle.AddedToEnterpriseAt
                        };

                        _vehiclesService.UpdateVehicle(vehicle, updateRequest);
                        
                        usedDriverIds.Add(availableDriver.Id);
                    }
                }
            }
        }
    }
                        
    private Guid GenerateDeterministicGuid()
    {
        byte[] bytes = new byte[16];
        _random.NextBytes(bytes);
        return new Guid(bytes);
    }
}