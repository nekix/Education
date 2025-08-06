using Bogus;
using CarPark.Models.Drivers;
using CarPark.Models.Enterprises;
using CarPark.Models.Models;
using CarPark.Models.Vehicles;

namespace CarPark.DataGenerator
{
    public class DataGenerator
    {
        private readonly Faker<Enterprise> _enterpriseFaker;
        private readonly Faker<Model> _modelFaker;
        private readonly Faker<Vehicle> _vehicleFaker;
        private readonly Faker<Driver> _driverFaker;

        public DataGenerator()
        {
            // Настройка генератора предприятий
            _enterpriseFaker = new Faker<Enterprise>("ru")
                .RuleFor(e => e.Id, f => f.IndexGlobal)
                .RuleFor(e => e.Name, f => f.Company.CompanyName())
                .RuleFor(e => e.LegalAddress, f => f.Address.FullAddress())
                .RuleFor(e => e.Managers, f => new List<CarPark.Models.Managers.Manager>());

            // Настройка генератора моделей
            _modelFaker = new Faker<Model>("ru")
                .RuleFor(m => m.Id, f => f.IndexGlobal)
                .RuleFor(m => m.ModelName, f => f.Vehicle.Model())
                .RuleFor(m => m.VehicleType, f => f.PickRandom("Легковой", "Грузовой", "Автобус", "Микроавтобус"))
                .RuleFor(m => m.SeatsCount, f => f.Random.Int(2, 50))
                .RuleFor(m => m.MaxLoadingWeightKg, f => f.Random.Double(500, 5000))
                .RuleFor(m => m.EnginePowerKW, f => f.Random.Double(50, 500))
                .RuleFor(m => m.TransmissionType, f => f.PickRandom("Механическая", "Автоматическая", "Робот", "Вариатор"))
                .RuleFor(m => m.FuelSystemType, f => f.PickRandom("Бензин", "Дизель", "Гибрид", "Электро"))
                .RuleFor(m => m.FuelTankVolumeLiters, f => f.Random.Int(30, 100).ToString());

            // Настройка генератора машин
            _vehicleFaker = new Faker<Vehicle>("ru")
                .RuleFor(v => v.Id, f => f.IndexGlobal)
                .RuleFor(v => v.ModelId, f => f.Random.Int(1, 20)) // Будет переопределено
                .RuleFor(v => v.EnterpriseId, f => f.Random.Int(1, 100)) // Будет переопределено
                .RuleFor(v => v.VinNumber, f => f.Vehicle.Vin())
                .RuleFor(v => v.Price, f => f.Random.Decimal(500000, 5000000))
                .RuleFor(v => v.ManufactureYear, f => f.Random.Int(2010, 2024))
                .RuleFor(v => v.Mileage, f => f.Random.Int(0, 300000))
                .RuleFor(v => v.Color, f => f.PickRandom("Белый", "Черный", "Серебристый", "Красный", "Синий", "Зеленый", "Серый"))
                .RuleFor(v => v.AssignedDrivers, f => new List<Driver>())
                .RuleFor(v => v.ActiveAssignedDriver, f => (Driver?)null);

            // Настройка генератора водителей
            _driverFaker = new Faker<Driver>("ru")
                .RuleFor(d => d.Id, f => f.IndexGlobal)
                .RuleFor(d => d.EnterpriseId, f => f.Random.Int(1, 100)) // Будет переопределено
                .RuleFor(d => d.FullName, f => f.Name.FullName())
                .RuleFor(d => d.DriverLicenseNumber, f => f.Random.Replace("## ## ######"))
                .RuleFor(d => d.AssignedVehicles, f => new List<Vehicle>())
                .RuleFor(d => d.ActiveAssignedVehicle, f => (Vehicle?)null);
        }

        public GeneratedData GenerateAll(int enterprisesCount, int vehiclesPerEnterprise)
        {
            var enterprises = _enterpriseFaker.Generate(enterprisesCount);
            var models = _modelFaker.Generate(20); // Генерируем 20 моделей
            var vehicles = new List<Vehicle>();
            var drivers = new List<Driver>();

            var driverId = 1;
            var vehicleId = 1;

            foreach (var enterprise in enterprises)
            {
                // Генерируем машины для предприятия
                var enterpriseVehicles = _vehicleFaker.Generate(vehiclesPerEnterprise);
                foreach (var vehicle in enterpriseVehicles)
                {
                    vehicle.Id = vehicleId++;
                    vehicle.EnterpriseId = enterprise.Id;
                    vehicle.ModelId = models[Random.Shared.Next(models.Count)].Id;
                    vehicles.Add(vehicle);
                }

                // Генерируем водителей для предприятия (примерно 1 водитель на 10 машин)
                var driversCount = Math.Max(1, vehiclesPerEnterprise / 10);
                var enterpriseDrivers = _driverFaker.Generate(driversCount);
                foreach (var driver in enterpriseDrivers)
                {
                    driver.Id = driverId++;
                    driver.EnterpriseId = enterprise.Id;
                    drivers.Add(driver);
                }

                // Назначаем водителей машинам (примерно каждая 10-я машина с активным водителем)
                var enterpriseVehiclesList = vehicles.Where(v => v.EnterpriseId == enterprise.Id).ToList();
                var enterpriseDriversList = drivers.Where(d => d.EnterpriseId == enterprise.Id).ToList();

                for (int i = 0; i < enterpriseVehiclesList.Count; i++)
                {
                    var vehicle = enterpriseVehiclesList[i];
                
                    // Каждая 10-я машина получает активного водителя
                    if (i % 10 == 0 && enterpriseDriversList.Any())
                    {
                        var driver = enterpriseDriversList[Random.Shared.Next(enterpriseDriversList.Count)];
                    
                        vehicle.ActiveAssignedDriver = driver;
                        vehicle.AssignedDrivers.Add(driver);
                    
                        driver.ActiveAssignedVehicle = vehicle;
                        driver.AssignedVehicles.Add(vehicle);
                    }
                }
            }

            return new GeneratedData
            {
                Enterprises = enterprises,
                Models = models,
                Vehicles = vehicles,
                Drivers = drivers
            };
        }
    }
}