using Bogus;
using CarPark.Models.Drivers;
using CarPark.Models.Models;
using CarPark.Models.Vehicles;

namespace CarPark.DataGenerator
{
    public class DataGenerator
    {
        private readonly Faker<Vehicle> _vehicleFaker;
        private readonly Faker<Driver> _driverFaker;
        
        public DataGenerator()
        {
            // Настройка генератора машин
            _vehicleFaker = new Faker<Vehicle>("ru")
                .RuleFor(v => v.Id, f => default)
                .RuleFor(v => v.VinNumber, f => f.Vehicle.Vin())
                .RuleFor(v => v.Price, f => f.Random.Decimal(500000, 5000000))
                .RuleFor(v => v.ManufactureYear, f => f.Random.Int(2010, 2024))
                .RuleFor(v => v.Mileage, f => f.Random.Int(0, 300000))
                .RuleFor(v => v.Color, f => f.PickRandom("Белый", "Черный", "Серебристый", "Красный", "Синий", "Зеленый", "Серый"))
                .RuleFor(v => v.AssignedDrivers, f => new List<Driver>())
                .RuleFor(v => v.ActiveAssignedDriver, f => (Driver?)null);

            // Настройка генератора водителей
            _driverFaker = new Faker<Driver>("ru")
                .RuleFor(d => d.Id, f => default)
                .RuleFor(d => d.FullName, f => f.Name.FullName())
                .RuleFor(d => d.DriverLicenseNumber, f => f.Random.Replace("## ## ######"))
                .RuleFor(d => d.AssignedVehicles, f => new List<Vehicle>())
                .RuleFor(d => d.ActiveAssignedVehicle, f => (Vehicle?)null);
        }

        /// <summary>
        /// Генерирует коллекцию автомобилей для указанного предприятия
        /// </summary>
        /// <param name="enterpriseId">ID предприятия</param>
        /// <param name="models">Список доступных моделей</param>
        /// <returns>Коллекция сгенерированных автомобилей</returns>
        public IEnumerable<Vehicle> GenerateVehicles(int enterpriseId, List<Model> models)
        {
            return _vehicleFaker
                .RuleFor(v => v.EnterpriseId, f => enterpriseId)
                .RuleFor(v => v.ModelId, f => f.PickRandom(models).Id)
                .GenerateForever();
        }

        /// <summary>
        /// Генерирует коллекцию водителей для указанного предприятия
        /// </summary>
        /// <param name="enterpriseId">ID предприятия</param>
        /// <returns>Коллекция сгенерированных водителей</returns>
        public IEnumerable<Driver> GenerateDrivers(int enterpriseId)
        {
            return _driverFaker
                .RuleFor(d => d.EnterpriseId, f => enterpriseId)
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
            Dictionary<int, List<Vehicle>> vehiclesByEnterprise = vehicles
                .GroupBy(v => v.EnterpriseId)
                .ToDictionary(g => g.Key, g => g.ToList());

            Dictionary<int, List<Driver>> driversByEnterprise = drivers
                .GroupBy(d => d.EnterpriseId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Обрабатываем каждое предприятие отдельно
            foreach (int enterpriseId in vehiclesByEnterprise.Keys.Intersect(driversByEnterprise.Keys))
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
                    if (Random.Shared.NextDouble() < assignmentRatio && enterpriseDrivers.Any())
                    {
                        // Случайное количество водителей на автомобиль
                        int selectedDriversCount = Random.Shared.Next(
                            minDriversPerVehicle, 
                            Math.Min(maxDriversPerVehicle + 1, enterpriseDrivers.Count + 1)
                        );
                        List<Driver> selectedDrivers = enterpriseDrivers
                            .OrderBy(x => Random.Shared.Next())
                            .Take(selectedDriversCount)
                            .ToList();

                        foreach (Driver driver in selectedDrivers)
                        {
                            if (!vehicle.AssignedDrivers.Contains(driver))
                            {
                                vehicle.AssignedDrivers.Add(driver);
                                driver.AssignedVehicles.Add(vehicle);
                            }
                        }
                    }
                }

                // 2. Назначаем активных водителей
                List<Vehicle> vehiclesWithDrivers = enterpriseVehicles
                    .Where(v => v.AssignedDrivers.Any())
                    .OrderBy(x => Random.Shared.Next())
                    .ToList();

                HashSet<int> usedDriverIds = new HashSet<int>();

                double fixedActiveDriverRation =
                    (double)enterpriseVehicles.Count / vehiclesWithDrivers.Count * activeDriverRatio;

                foreach (Vehicle vehicle in vehiclesWithDrivers)
                {
                    // Применяем коэффициент активных водителей
                    if (Random.Shared.NextDouble() < fixedActiveDriverRation)
                    {
                        Driver? availableDriver = vehicle.AssignedDrivers
                            .FirstOrDefault(d => !usedDriverIds.Contains(d.Id));

                        if (availableDriver != null)
                        {
                            vehicle.ActiveAssignedDriver = availableDriver;
                            availableDriver.ActiveAssignedVehicle = vehicle;
                            usedDriverIds.Add(availableDriver.Id);
                        }
                    }
                }
            }
        }
    }
}
