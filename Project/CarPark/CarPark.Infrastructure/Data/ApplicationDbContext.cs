using CarPark.Data.Interfaces;
using CarPark.DateTimes;
using CarPark.Drivers;
using CarPark.Enterprises;
using CarPark.Managers;
using CarPark.Models;
using CarPark.Rides;
using CarPark.TimeZones;
using CarPark.Vehicles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Data;

public class ApplicationDbContext : IdentityDbContext, IModelsDbSet, IVehiclesDbSet, IEnterprisesDbSet, IVehicleGeoTimePointsDbSet, IRidesDbSet
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<Vehicle> Vehicles { get; set; } = default!;
    public DbSet<Model> Models { get; set; } = default!;
    public DbSet<Driver> Drivers { get; set; } = default!;
    public DbSet<Enterprise> Enterprises { get; set; } = default!;
    public DbSet<Manager> Managers { get; set; } = default!;
    public DbSet<TzInfo> TzInfos { get; set; } = default!;
    public DbSet<VehicleGeoTimePoint> VehicleGeoTimePoints { get; set; } = default!;
    public DbSet<Ride> Rides { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureModel();
        modelBuilder.ConfigureVehicle();
        modelBuilder.ConfigureDriver();
        modelBuilder.ConfigureManager();
        modelBuilder.ConfigureEnterprise();
        modelBuilder.ConfigureTzInfo();
        modelBuilder.ConfigureVehicleGeoTimePoint();
        modelBuilder.ConfigureRides();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.UseUtcDateTimeOffset();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSeeding((context, _) =>
        {
            if (!context.Set<IdentityUser>().Any())
            {
                context.Set<IdentityUser>().AddRange(SeedIdentityUsers());
                context.SaveChanges();
            }
        });

        optionsBuilder.UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
            if (!await context.Set<IdentityUser>().AnyAsync())
            {
                await context.Set<IdentityUser>().AddRangeAsync(SeedIdentityUsers());
                await context.SaveChangesAsync();
            }
        });
    }

    //optionsBuilder.UseSeeding((context, _) =>
    //{
    //    if (!CheckHasAnyData(context))
    //    {
    //        context.Set<TzInfo>().AddRange(GetSeedTzInfos(context, _localIcuTimezoneService));
    //        context.SaveChanges();

    //        context.Set<Model>().AddRange(GetSeedModels());
    //        context.SaveChanges();

    //        context.Set<Enterprise>().AddRange(GetSeedEnterprises());
    //        context.SaveChanges();

    //        context.Set<Vehicle>().AddRange(GetSeedVehicles());
    //        context.SaveChanges();

    //        context.Set<Driver>().AddRange(GetSeedDrivers(context));
    //        context.SaveChanges();

    //        context.Set<IdentityUser>().AddRange(SeedIdentityUsers());
    //        context.SaveChanges();

    //        context.Set<Manager>().AddRange(GetSeedManagers(context));
    //        context.SaveChanges();

    //        context.Set<VehicleGeoTimePoint>().AddRange(GetSeedVehicleGeoTimePoints(context));
    //        context.SaveChanges();
    //    }
    //});

    //optionsBuilder.UseAsyncSeeding(async (context, _, cancellationToken) => 
    //{
    //    if (!CheckHasAnyData(context))
    //    {
    //        await context.Set<TzInfo>().AddRangeAsync(GetSeedTzInfos(context, _localIcuTimezoneService), cancellationToken);
    //        await context.SaveChangesAsync(cancellationToken);

    //        await context.Set<Model>().AddRangeAsync(GetSeedModels(), cancellationToken);
    //        await context.SaveChangesAsync(cancellationToken);

    //        await context.Set<Enterprise>().AddRangeAsync(GetSeedEnterprises(), cancellationToken);
    //        await context.SaveChangesAsync(cancellationToken);

    //        await context.Set<Vehicle>().AddRangeAsync(GetSeedVehicles(), cancellationToken);
    //        await context.SaveChangesAsync(cancellationToken);

    //        await context.Set<Driver>().AddRangeAsync(GetSeedDrivers(context), cancellationToken);
    //        await context.SaveChangesAsync(cancellationToken);

    //        await context.Set<IdentityUser>().AddRangeAsync(SeedIdentityUsers(), cancellationToken);
    //        await context.SaveChangesAsync(cancellationToken);

    //        await context.Set<Manager>().AddRangeAsync(GetSeedManagers(context), cancellationToken);
    //        await context.SaveChangesAsync(cancellationToken);

    //        await context.Set<VehicleGeoTimePoint>().AddRangeAsync(GetSeedVehicleGeoTimePoints(context), cancellationToken);
    //        await context.SaveChangesAsync(cancellationToken);
    //    }
    //});

    //private static bool CheckHasAnyData(DbContext context)
    //{
    //    return context.Set<Model>().Any()
    //           || context.Set<Vehicle>().Any()
    //           || context.Set<Enterprise>().Any()
    //           || context.Set<Driver>().Any()
    //           || context.Set<IdentityUser>().Any()
    //           || context.Set<Manager>().Any()
    //           || context.Set<VehicleGeoTimePoint>().Any();
    //}

    //private static IReadOnlyList<Model> GetSeedModels()
    //{
    //    List<Model> models = new List<Model>
    //            {
    //                // NoName модель
    //                new Model()
    //                {
    //                    Id = default,
    //                    ModelName = "NoName",
    //                    VehicleType = string.Empty,
    //                    SeatsCount = default,
    //                    MaxLoadingWeightKg = default,
    //                    EnginePowerKW = default,
    //                    TransmissionType = string.Empty,
    //                    FuelSystemType = string.Empty,
    //                    FuelTankVolumeLiters = string.Empty
    //                },
    //                // Легковые автомобили
    //                new Model
    //                {
    //                    Id = default,
    //                    ModelName = "Toyota Camry",
    //                    VehicleType = "Легковой автомобиль",
    //                    SeatsCount = 5,
    //                    MaxLoadingWeightKg = 500,
    //                    EnginePowerKW = 150,
    //                    TransmissionType = "Автомат",
    //                    FuelSystemType = "Бензин",
    //                    FuelTankVolumeLiters = "60"
    //                },
    //                // Грузовые автомобили
    //                new Model
    //                {
    //                    Id = default,
    //                    ModelName = "Газель Next",
    //                    VehicleType = "Грузовой автомобиль",
    //                    SeatsCount = 3,
    //                    MaxLoadingWeightKg = 2000,
    //                    EnginePowerKW = 110,
    //                    TransmissionType = "Механика",
    //                    FuelSystemType = "Дизель",
    //                    FuelTankVolumeLiters = "80"
    //                },
    //                // Автобусы
    //                new Model
    //                {
    //                    Id = default,
    //                    ModelName = "Mercedes-Benz Sprinter",
    //                    VehicleType = "Автобус",
    //                    SeatsCount = 19,
    //                    MaxLoadingWeightKg = 1500,
    //                    EnginePowerKW = 120,
    //                    TransmissionType = "Автомат",
    //                    FuelSystemType = "Дизель",
    //                    FuelTankVolumeLiters = "75"
    //                },
    //                // Мотоциклы
    //                new Model
    //                {
    //                    Id = default,
    //                    ModelName = "Honda CB300R",
    //                    VehicleType = "Мотоцикл",
    //                    SeatsCount = 2,
    //                    MaxLoadingWeightKg = 180,
    //                    EnginePowerKW = 20,
    //                    TransmissionType = "Механика",
    //                    FuelSystemType = "Бензин",
    //                    FuelTankVolumeLiters = "10"
    //                },
    //                // Тяжелые грузовики
    //                new Model
    //                {
    //                    Id = default,
    //                    ModelName = "КАМАЗ 5320",
    //                    VehicleType = "Грузовой автомобиль",
    //                    SeatsCount = 3,
    //                    MaxLoadingWeightKg = 8000,
    //                    EnginePowerKW = 180,
    //                    TransmissionType = "Механика",
    //                    FuelSystemType = "Дизель",
    //                    FuelTankVolumeLiters = "350"
    //                },
    //                // Тяжелые автобусы
    //                new Model
    //                {
    //                    Id = default,
    //                    ModelName = "ПАЗ 3205",
    //                    VehicleType = "Автобус",
    //                    SeatsCount = 41,
    //                    MaxLoadingWeightKg = 2000,
    //                    EnginePowerKW = 90,
    //                    TransmissionType = "Механика",
    //                    FuelSystemType = "Дизель",
    //                    FuelTankVolumeLiters = "105"
    //                }
    //            };

    //    return models;
    //}

    //private static IReadOnlyList<Vehicle> GetSeedVehicles()
    //{
    //    List<Vehicle> vehicles = new List<Vehicle>
    //    {
    //        // Легковые автомобили
    //        new Vehicle
    //        {
    //            Id = default,
    //            AssignedDrivers = new List<Driver>(0),
    //            ActiveAssignedDriver = null,
    //            ModelId = 1, 
    //            EnterpriseId = 1,
    //            VinNumber = "VIN0001", 
    //            Price = 10000, 
    //            ManufactureYear = 2015, 
    //            Mileage = 50000,
    //            Color = "Красный",
    //            AddedToEnterpriseAt = DateTimeOffset.Parse("13/10/2024 13:02:03 00:00")
    //        },
    //        new Vehicle
    //        {
    //            Id = default,
    //            AssignedDrivers = new List<Driver>(0),
    //            ActiveAssignedDriver = null,
    //            ModelId = 1, 
    //            EnterpriseId = 1,
    //            VinNumber = "VIN0002", 
    //            Price = 12000, 
    //            ManufactureYear = 2017, 
    //            Mileage = 30000,
    //            Color = "Синий",
    //            AddedToEnterpriseAt = DateTimeOffset.Parse("13/10/2024 13:02:03 00:00")
    //        },
    //        // Грузовые автомобили
    //        new Vehicle
    //        {
    //            Id = default,
    //            AssignedDrivers = new List<Driver>(0),
    //            ActiveAssignedDriver = null,
    //            ModelId = 3, 
    //            EnterpriseId = 1,
    //            VinNumber = "VIN0005", 
    //            Price = 11000, 
    //            ManufactureYear = 2016, 
    //            Mileage = 45000,
    //            Color = "Зелёный",
    //            AddedToEnterpriseAt = DateTimeOffset.Parse("13/10/2024 13:02:03 00:00")
    //        },
    //        new Vehicle
    //        {
    //            Id = default,
    //            AssignedDrivers = new List<Driver>(0),
    //            ActiveAssignedDriver = null,
    //            ModelId = 2, 
    //            EnterpriseId = 2,
    //            VinNumber = "VIN0003", 
    //            Price = 9000, 
    //            ManufactureYear = 2013, 
    //            Mileage = 70000,
    //            Color = "Чёрный",
    //            AddedToEnterpriseAt = DateTimeOffset.Parse("13/10/2024 13:02:03 00:00")
    //        },
    //        new Vehicle
    //        {
    //            Id = default,
    //            AssignedDrivers = new List<Driver>(0),
    //            ActiveAssignedDriver = null,
    //            ModelId = 2, 
    //            EnterpriseId = 2,
    //            VinNumber = "VIN0004", 
    //            Price = 15000, 
    //            ManufactureYear = 2019, 
    //            Mileage = 20000,
    //            Color = "Белый",
    //            AddedToEnterpriseAt = DateTimeOffset.Parse("13/10/2024 13:02:03 00:00")
    //        },
    //        new Vehicle
    //        {
    //            Id = default,
    //            AssignedDrivers = new List<Driver>(0),
    //            ActiveAssignedDriver = null,
    //            ModelId = 5, 
    //            EnterpriseId = 2,
    //            VinNumber = "VIN0009", 
    //            Price = 12500, 
    //            ManufactureYear = 2017, 
    //            Mileage = 40000,
    //            Color = "Коричневый",
    //            AddedToEnterpriseAt = DateTimeOffset.Parse("13/10/2024 13:02:03 00:00")
    //        },
    //        // Мотоциклы
    //        new Vehicle
    //        {
    //            Id = default,
    //            AssignedDrivers = new List<Driver>(0),
    //            ActiveAssignedDriver = null,
    //            ModelId = 4, 
    //            EnterpriseId = 3,
    //            VinNumber = "VIN0007", 
    //            Price = 9500, 
    //            ManufactureYear = 2014, 
    //            Mileage = 60000,
    //            Color = "Серый",
    //            AddedToEnterpriseAt = DateTimeOffset.Parse("13/10/2024 13:02:03 00:00")
    //        },
    //        new Vehicle
    //        {
    //            Id = default,
    //            AssignedDrivers = new List<Driver>(0),
    //            ActiveAssignedDriver = null,
    //            ModelId = 4, 
    //            EnterpriseId = 3,
    //            VinNumber = "VIN0008", 
    //            Price = 14000, 
    //            ManufactureYear = 2020, 
    //            Mileage = 15000,
    //            Color = "Бежевый",
    //            AddedToEnterpriseAt = DateTimeOffset.Parse("13/10/2024 13:02:03 00:00")
    //        },
    //        new Vehicle
    //        {
    //            Id = default,
    //            AssignedDrivers = new List<Driver>(0),
    //            ActiveAssignedDriver = null,
    //            ModelId = 3, 
    //            EnterpriseId = 3,
    //            VinNumber = "VIN0006", 
    //            Price = 13000, 
    //            ManufactureYear = 2018, 
    //            Mileage = 35000,
    //            Color = "Серебристый",
    //            AddedToEnterpriseAt = DateTimeOffset.Parse("13/10/2024 13:02:03 00:00")
    //        },
    //        // Тяжелые автобусы
    //        new Vehicle
    //        {
    //            Id = default,
    //            AssignedDrivers = new List<Driver>(0),
    //            ActiveAssignedDriver = null,
    //            ModelId = 6, 
    //            EnterpriseId = 3,
    //            VinNumber = "VIN0010", 
    //            Price = 16000, 
    //            ManufactureYear = 2021, 
    //            Mileage = 10000,
    //            Color = "Фиолетовый",
    //            AddedToEnterpriseAt = DateTimeOffset.Parse("13/10/2024 13:02:03 00:00")
    //        }
    //    };

    //    return vehicles;
    //}

    //private static IReadOnlyList<Enterprise> GetSeedEnterprises()
    //{
    //    List<Enterprise> enterprises = new List<Enterprise>
    //    {
    //        new Enterprise
    //        {
    //            Id = default,
    //            Managers = new List<Manager>(0),
    //            Name = "Извозкин.Такси-Парк",
    //            LegalAddress = "г. Москва, ул. Цифровая, д. 154",
    //            TimeZone = null
    //        },
    //        new Enterprise
    //        {
    //            Id = default,
    //            Managers = new List<Manager>(0),
    //            Name = "Delivery-Express",
    //            LegalAddress = "г. Санкт-Петербург, пр. Курьерский, д. 15",
    //            TimeZone = null
    //        },
    //        new Enterprise
    //        {
    //            Id = default,
    //            Managers = new List<Manager>(0),
    //            Name = "DoStavka",
    //            LegalAddress = "г. Москва, ул. Зеленая, д. 77",
    //            TimeZone = null
    //        },
    //        new Enterprise
    //        {
    //            Id = default,
    //            Managers = new List<Manager>(0),
    //            Name = "Red.Такси",
    //            LegalAddress = "г. Москва, ул. Красная, д. 21",
    //            TimeZone = null
    //        }
    //    };

    //    return enterprises;
    //}

    //private static IReadOnlyList<Driver> GetSeedDrivers(DbContext context)
    //{
    //    List<Driver> drivers = new List<Driver>
    //    {
    //        // Водители для Извозкин.Такси-Парк (Enterprise Id = 1)
    //        new Driver
    //        {
    //            Id = default,
    //            AssignedVehicles = new List<Vehicle>(0),
    //            ActiveAssignedVehicle = null,
    //            EnterpriseId = 1,
    //            FullName = "Иванов Иван Иванович",
    //            DriverLicenseNumber = "7777 123456"
    //        },
    //        new Driver
    //        {
    //            Id = default,
    //            AssignedVehicles = new List<Vehicle>(0),
    //            ActiveAssignedVehicle = null,
    //            EnterpriseId = 1,
    //            FullName = "Петров Петр Петрович",
    //            DriverLicenseNumber = "7777 234567"
    //        },
    //        new Driver
    //        {
    //            Id = default,
    //            AssignedVehicles = new List<Vehicle>(0),
    //            ActiveAssignedVehicle = null,
    //            EnterpriseId = 1,
    //            FullName = "Сидоров Сидор Сидорович",
    //            DriverLicenseNumber = "7777 345678"
    //        },
    //        // Водители для Delivery-Express (Enterprise Id = 2)
    //        new Driver
    //        {
    //            Id = default,
    //            AssignedVehicles = new List<Vehicle>(0),
    //            ActiveAssignedVehicle = null,
    //            EnterpriseId = 2,
    //            FullName = "Александров Александр Александрович",
    //            DriverLicenseNumber = "7778 123456"
    //        },
    //        new Driver
    //        {
    //            Id = default,
    //            AssignedVehicles = new List<Vehicle>(0),
    //            ActiveAssignedVehicle = null,
    //            EnterpriseId = 2,
    //            FullName = "Михайлов Михаил Михайлович",
    //            DriverLicenseNumber = "7778 234567"
    //        },
    //        new Driver
    //        {
    //            Id = default,
    //            AssignedVehicles = new List<Vehicle>(0),
    //            ActiveAssignedVehicle = null,
    //            EnterpriseId = 2,
    //            FullName = "Николаев Николай Николаевич",
    //            DriverLicenseNumber = "7778 345678"
    //        },
    //        // Водители для DoStavka (Enterprise Id = 3)
    //        new Driver
    //        {
    //            Id = default,
    //            AssignedVehicles = new List<Vehicle>(0),
    //            ActiveAssignedVehicle = null,
    //            EnterpriseId = 3,
    //            FullName = "Сергеев Сергей Сергеевич",
    //            DriverLicenseNumber = "7779 123456"
    //        },
    //        new Driver
    //        {
    //            Id = default,
    //            AssignedVehicles = new List<Vehicle>(0),
    //            ActiveAssignedVehicle = null,
    //            EnterpriseId = 3,
    //            FullName = "Андреев Андрей Андреевич",
    //            DriverLicenseNumber = "7779 234567"
    //        }
    //    };

    //    // Get existing vehicles from context to avoid tracking conflicts
    //    List<Vehicle> existingVehicles = context.Set<Vehicle>().ToList();

    //    drivers[0].AssignedVehicles = new List<Vehicle>
    //    {
    //        existingVehicles.First(v => v.Id == 1),
    //        existingVehicles.First(v => v.Id == 2),
    //    };
    //    drivers[0].ActiveAssignedVehicle = existingVehicles.First(v => v.Id == 1);

    //    drivers[1].AssignedVehicles = new List<Vehicle>
    //    {
    //        existingVehicles.First(v => v.Id == 1),
    //        existingVehicles.First(v => v.Id == 2),
    //    };

    //    drivers[3].AssignedVehicles = new List<Vehicle>
    //    {
    //        existingVehicles.First(v => v.Id == 4),
    //        existingVehicles.First(v => v.Id == 5),
    //    };
    //    drivers[3].ActiveAssignedVehicle = existingVehicles.First(v => v.Id == 4);

    //    drivers[4].AssignedVehicles = new List<Vehicle>
    //    {
    //        existingVehicles.First(v => v.Id == 4),
    //        existingVehicles.First(v => v.Id == 5),
    //    };

    //    drivers[6].AssignedVehicles = new List<Vehicle>
    //    {
    //        existingVehicles.First(v => v.Id == 7),
    //        existingVehicles.First(v => v.Id == 8),
    //    };
    //    drivers[6].ActiveAssignedVehicle = existingVehicles.First(v => v.Id == 7);

    //    drivers[7].AssignedVehicles = new List<Vehicle>
    //    {
    //        existingVehicles.First(v => v.Id == 9),
    //        existingVehicles.First(v => v.Id == 10),
    //    };
    //    drivers[7].ActiveAssignedVehicle = existingVehicles.First(v => v.Id == 9);

    //    return drivers;
    //}

    //private static IReadOnlyList<IdentityUser> SeedIdentityUsers()
    //{
    //    PasswordHasher<IdentityUser> hasher = new PasswordHasher<IdentityUser>();

    //    List<IdentityUser> users = new List<IdentityUser>();

    //    IdentityUser adminUser = new IdentityUser
    //    {
    //        Id = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    //        UserName = "manager1",
    //        NormalizedUserName = "MANAGER1",
    //        SecurityStamp = Guid.NewGuid().ToString(),
    //        ConcurrencyStamp = Guid.NewGuid().ToString()
    //    };
    //    adminUser.PasswordHash = hasher.HashPassword(adminUser, "123456");
    //    users.Add(adminUser);

    //    IdentityUser managerUser = new IdentityUser
    //    {
    //        Id = "b2c3d4e5-f6g7-8901-bcde-f23456789012",
    //        UserName = "manager2",
    //        NormalizedUserName = "MANAGER2",
    //        SecurityStamp = Guid.NewGuid().ToString(),
    //        ConcurrencyStamp = Guid.NewGuid().ToString()
    //    };
    //    managerUser.PasswordHash = hasher.HashPassword(managerUser, "123456");
    //    users.Add(managerUser);

    //    return users;
    //}

    //private static IReadOnlyList<Manager> GetSeedManagers(DbContext context)
    //{
    //    List<Enterprise> existingEnterprises = context.Set<Enterprise>().ToList();

    //    List<Manager> managers = new List<Manager>
    //    {
    //        new Manager
    //        {
    //            Id = default,
    //            IdentityUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    //            Enterprises = existingEnterprises.Where(e => e.Id == 1 || e.Id == 2).ToList()
    //        },
    //        new Manager
    //        {
    //            Id = default,
    //            IdentityUserId = "b2c3d4e5-f6g7-8901-bcde-f23456789012", 
    //            Enterprises = existingEnterprises.Where(e => e.Id == 2 || e.Id == 3).ToList()
    //        }
    //    };

    //    return managers;
    //}

    //private static IReadOnlyList<TzInfo> GetSeedTzInfos(DbContext context, LocalIcuTimezoneService timezoneService)
    //{
    //    IReadOnlyCollection<string> ianaIds = timezoneService.GetAvailableIanaIds();

    //    return timezoneService
    //        .MapIanaIdsToWindowsIds(ianaIds)
    //        .Select(x => new TzInfo(x.Key, x.Value))
    //        .ToList()
    //        .AsReadOnly();
    //}

    //private static IReadOnlyList<VehicleGeoTimePoint> GetSeedVehicleGeoTimePoints(DbContext context)
    //{
    //    List<VehicleGeoTimePoint> geoTimePoints = new List<VehicleGeoTimePoint>();
    //    List<Vehicle> existingVehicles = context.Set<Vehicle>().OrderBy(v => v.Id).Take(5).ToList();

    //    if (existingVehicles.Count == 0)
    //        return geoTimePoints.AsReadOnly();

    //    DateTimeOffset baseTime = DateTimeOffset.UtcNow.AddHours(-4);
    //    int intervalSeconds = 10;
    //    int totalDurationHours = 3;
    //    int totalPoints = (totalDurationHours * 3600) / intervalSeconds;

    //    // Define different routes for each vehicle
    //    List<VehicleRoute> routes = new List<VehicleRoute>
    //    {
    //        // Vehicle 1: Route around Moscow center
    //        new VehicleRoute
    //        {
    //            StartLat = 55.7558, StartLon = 37.6176,
    //            EndLat = 55.7522, EndLon = 37.6156,
    //            RoutePoints = new[]
    //            {
    //                new { Lat = 55.7558, Lon = 37.6176 },
    //                new { Lat = 55.7539, Lon = 37.6208 },
    //                new { Lat = 55.7520, Lon = 37.6175 },
    //                new { Lat = 55.7505, Lon = 37.6142 },
    //                new { Lat = 55.7522, Lon = 37.6156 } 
    //            }
    //        },
    //        // Vehicle 2: Route in business district
    //        new VehicleRoute
    //        {
    //            StartLat = 55.7490, StartLon = 37.5380,
    //            EndLat = 55.7601, EndLon = 37.5499,
    //            RoutePoints = new[]
    //            {
    //                new { Lat = 55.7490, Lon = 37.5380 },
    //                new { Lat = 55.7520, Lon = 37.5420 }, 
    //                new { Lat = 55.7550, Lon = 37.5450 },
    //                new { Lat = 55.7580, Lon = 37.5480 },
    //                new { Lat = 55.7601, Lon = 37.5499 }
    //            }
    //        },
    //        // Vehicle 3: Route to airport
    //        new VehicleRoute
    //        {
    //            StartLat = 55.7558, StartLon = 37.6176,
    //            EndLat = 55.9728, EndLon = 37.4147,
    //            RoutePoints = new[]
    //            {
    //                new { Lat = 55.7558, Lon = 37.6176 },
    //                new { Lat = 55.8000, Lon = 37.5800 },
    //                new { Lat = 55.8500, Lon = 37.5400 },
    //                new { Lat = 55.9200, Lon = 37.4800 },
    //                new { Lat = 55.9728, Lon = 37.4147 }
    //            }
    //        },
    //        // Vehicle 4: Circular route
    //        new VehicleRoute
    //        {
    //            StartLat = 55.7400, StartLon = 37.6200,
    //            EndLat = 55.7400, EndLon = 37.6200,
    //            RoutePoints = new[]
    //            {
    //                new { Lat = 55.7400, Lon = 37.6200 },
    //                new { Lat = 55.7450, Lon = 37.6300 },
    //                new { Lat = 55.7500, Lon = 37.6250 },
    //                new { Lat = 55.7450, Lon = 37.6150 },
    //                new { Lat = 55.7400, Lon = 37.6200 }
    //            }
    //        },
    //        // Vehicle 5: Delivery route
    //        new VehicleRoute
    //        {
    //            StartLat = 55.7300, StartLon = 37.6000,
    //            EndLat = 55.7800, EndLon = 37.6500,
    //            RoutePoints = new[]
    //            {
    //                new { Lat = 55.7300, Lon = 37.6000 },
    //                new { Lat = 55.7350, Lon = 37.6100 },
    //                new { Lat = 55.7450, Lon = 37.6200 },
    //                new { Lat = 55.7650, Lon = 37.6350 },
    //                new { Lat = 55.7800, Lon = 37.6500 }
    //            }
    //        }
    //    };

    //    // Generate points for each vehicle
    //    //for (int vehicleIndex = 0; vehicleIndex < Math.Min(existingVehicles.Count, routes.Count); vehicleIndex++)
    //    //{
    //    //    Vehicle vehicle = existingVehicles[vehicleIndex];
    //    //    VehicleRoute route = routes[vehicleIndex];

    //    //    for (int pointIndex = 0; pointIndex < totalPoints; pointIndex++)
    //    //    {
    //    //        DateTimeOffset currentTime = baseTime.AddSeconds(pointIndex * intervalSeconds);

    //    //        // Calculate position along route
    //    //        double progress = (double)pointIndex / (totalPoints - 1);
    //    //        int segmentIndex = (int)(progress * (route.RoutePoints.Length - 1));
    //    //        double segmentProgress = (progress * (route.RoutePoints.Length - 1)) - segmentIndex;

    //    //        if (segmentIndex >= route.RoutePoints.Length - 1)
    //    //        {
    //    //            segmentIndex = route.RoutePoints.Length - 2;
    //    //            segmentProgress = 1.0;
    //    //        }

    //    //        // Interpolate between route points
    //    //        double lat = route.RoutePoints[segmentIndex].Lat + 
    //    //            (route.RoutePoints[segmentIndex + 1].Lat - route.RoutePoints[segmentIndex].Lat) * segmentProgress;
    //    //        double lon = route.RoutePoints[segmentIndex].Lon + 
    //    //            (route.RoutePoints[segmentIndex + 1].Lon - route.RoutePoints[segmentIndex].Lon) * segmentProgress;

    //    //        // Add some random variation to make movement more realistic
    //    //        Random random = new Random(vehicleIndex * 1000 + pointIndex);
    //    //        lat += (random.NextDouble() - 0.5) * 0.001; // ±50 meters
    //    //        lon += (random.NextDouble() - 0.5) * 0.001; // ±50 meters

    //    //        NetTopologySuite.Geometries.Point point = new NetTopologySuite.Geometries.Point(lon, lat) { SRID = 4326 };

    //    //        VehicleGeoTimePoint geoTimePoint = new VehicleGeoTimePoint(
    //    //            Guid.NewGuid(),
    //    //            vehicle,
    //    //            point,
    //    //            new UtcDateTimeOffset(currentTime.ToUniversalTime())
    //    //        );

    //    //        geoTimePoints.Add(geoTimePoint);
    //    //    }
    //    //}

    //    return geoTimePoints.AsReadOnly();
    //}

    //private class VehicleRoute
    //{
    //    public double StartLat { get; set; }
    //    public double StartLon { get; set; }
    //    public double EndLat { get; set; }
    //    public double EndLon { get; set; }
    //    public dynamic[] RoutePoints { get; set; } = Array.Empty<dynamic>();
    //}
    private static IReadOnlyList<IdentityUser> SeedIdentityUsers()
    {
        PasswordHasher<IdentityUser> hasher = new PasswordHasher<IdentityUser>();

        List<IdentityUser> users = new List<IdentityUser>();

        IdentityUser adminUser = new IdentityUser
        {
            Id = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
            UserName = "manager1",
            NormalizedUserName = "MANAGER1",
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        adminUser.PasswordHash = hasher.HashPassword(adminUser, "123456");
        users.Add(adminUser);

        IdentityUser managerUser = new IdentityUser
        {
            Id = "b2c3d4e5-f6g7-8901-bcde-f23456789012",
            UserName = "manager2",
            NormalizedUserName = "MANAGER2",
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        managerUser.PasswordHash = hasher.HashPassword(managerUser, "123456");
        users.Add(managerUser);

        return users;
    }
}