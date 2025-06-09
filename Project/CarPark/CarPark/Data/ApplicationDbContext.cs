using CarPark.Models;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles { get; set; } = default!;
    public DbSet<Model> Models { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicle>()
            .ToTable("vehicle");

        modelBuilder.Entity<Vehicle>()
            .Property(v => v.Id)
            .UseIdentityAlwaysColumn();

        modelBuilder.Entity<Model>()
            .ToTable("model");

        modelBuilder.Entity<Model>()
            .HasData(CarPark.Models.Model.NoName);

        modelBuilder.Entity<Model>()
            .Property(m => m.Id)
            .UseIdentityAlwaysColumn();

        modelBuilder.Entity<Model>()
            .HasMany<Vehicle>()
            .WithOne()
            .HasForeignKey(v => v.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSeeding((context, _) =>
        {
            if (!context.Set<Model>().Any())
            {
                context.Set<Model>().AddRange(GetSeedModels());
                context.SaveChanges();

                if (!context.Set<Vehicle>().Any())
                {
                    context.Set<Vehicle>().AddRange(GetSeedVehicles());

                    context.SaveChanges();
                }
            }
        });

        optionsBuilder.UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
            if (!await context.Set<Model>().AnyAsync(cancellationToken))
            {
                await context.Set<Model>().AddRangeAsync(GetSeedModels(), cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                if (!await context.Set<Vehicle>().AnyAsync(cancellationToken))
                {
                    await context.Set<Vehicle>().AddRangeAsync(GetSeedVehicles(), cancellationToken);

                    await context.SaveChangesAsync(cancellationToken);
                }
            }
        });
    }

    private IReadOnlyList<Model> GetSeedModels()
    {
        List<Model> models = new List<Model>
                {
                    // Легковые автомобили
                    new Model
                    {
                        ModelName = "Toyota Camry",
                        VehicleType = "Легковой автомобиль",
                        SeatsCount = 5,
                        MaxLoadingWeightKg = 500,
                        EnginePowerKW = 150,
                        TransmissionType = "Автомат",
                        FuelSystemType = "Бензин",
                        FuelTankVolumeLiters = "60"
                    },
                    // Грузовые автомобили
                    new Model
                    {
                        ModelName = "Газель Next",
                        VehicleType = "Грузовой автомобиль",
                        SeatsCount = 3,
                        MaxLoadingWeightKg = 2000,
                        EnginePowerKW = 110,
                        TransmissionType = "Механика",
                        FuelSystemType = "Дизель",
                        FuelTankVolumeLiters = "80"
                    },
                    // Автобусы
                    new Model
                    {
                        ModelName = "Mercedes-Benz Sprinter",
                        VehicleType = "Автобус",
                        SeatsCount = 19,
                        MaxLoadingWeightKg = 1500,
                        EnginePowerKW = 120,
                        TransmissionType = "Автомат",
                        FuelSystemType = "Дизель",
                        FuelTankVolumeLiters = "75"
                    },
                    // Мотоциклы
                    new Model
                    {
                        ModelName = "Honda CB300R",
                        VehicleType = "Мотоцикл",
                        SeatsCount = 2,
                        MaxLoadingWeightKg = 180,
                        EnginePowerKW = 20,
                        TransmissionType = "Механика",
                        FuelSystemType = "Бензин",
                        FuelTankVolumeLiters = "10"
                    },
                    // Тяжелые грузовики
                    new Model
                    {
                        ModelName = "КАМАЗ 5320",
                        VehicleType = "Грузовой автомобиль",
                        SeatsCount = 3,
                        MaxLoadingWeightKg = 8000,
                        EnginePowerKW = 180,
                        TransmissionType = "Механика",
                        FuelSystemType = "Дизель",
                        FuelTankVolumeLiters = "350"
                    },
                    // Тяжелые автобусы
                    new Model
                    {
                        ModelName = "ПАЗ 3205",
                        VehicleType = "Автобус",
                        SeatsCount = 41,
                        MaxLoadingWeightKg = 2000,
                        EnginePowerKW = 90,
                        TransmissionType = "Механика",
                        FuelSystemType = "Дизель",
                        FuelTankVolumeLiters = "105"
                    }
                };

        return models;
    }

    private IReadOnlyList<Vehicle> GetSeedVehicles()
    {
        List<Vehicle> vehicles = new List<Vehicle>
        {
            // Легковые автомобили
            new Vehicle
            {
                ModelId = 1, VinNumber = "VIN0001", Price = 10000, ManufactureYear = 2015, Mileage = 50000,
                Color = "Красный"
            },
            new Vehicle
            {
                ModelId = 1, VinNumber = "VIN0002", Price = 12000, ManufactureYear = 2017, Mileage = 30000,
                Color = "Синий"
            },
            // Грузовые автомобили
            new Vehicle
            {
                ModelId = 2, VinNumber = "VIN0003", Price = 9000, ManufactureYear = 2013, Mileage = 70000,
                Color = "Чёрный"
            },
            new Vehicle
            {
                ModelId = 2, VinNumber = "VIN0004", Price = 15000, ManufactureYear = 2019, Mileage = 20000,
                Color = "Белый"
            },
            // Автобусы
            new Vehicle
            {
                ModelId = 3, VinNumber = "VIN0005", Price = 11000, ManufactureYear = 2016, Mileage = 45000,
                Color = "Зелёный"
            },
            new Vehicle
            {
                ModelId = 3, VinNumber = "VIN0006", Price = 13000, ManufactureYear = 2018, Mileage = 35000,
                Color = "Серебристый"
            },
            // Мотоциклы
            new Vehicle
            {
                ModelId = 4, VinNumber = "VIN0007", Price = 9500, ManufactureYear = 2014, Mileage = 60000,
                Color = "Серый"
            },
            new Vehicle
            {
                ModelId = 4, VinNumber = "VIN0008", Price = 14000, ManufactureYear = 2020, Mileage = 15000,
                Color = "Бежевый"
            },
            // Тяжелые грузовики
            new Vehicle
            {
                ModelId = 5, VinNumber = "VIN0009", Price = 12500, ManufactureYear = 2017, Mileage = 40000,
                Color = "Коричневый"
            },
            // Тяжелые автобусы
            new Vehicle
            {
                ModelId = 6, VinNumber = "VIN0010", Price = 16000, ManufactureYear = 2021, Mileage = 10000,
                Color = "Фиолетовый"
            }
        };

        return vehicles;
    }
}