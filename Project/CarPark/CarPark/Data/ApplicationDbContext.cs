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
    public DbSet<Driver> Drivers { get; set; } = default!;
    public DbSet<Enterprise> Enterprises { get; set; } = default!;
    public DbSet<DriverVehicleAssignment> DriverVehicleAssignments { get; set; } = default!;

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
            .Property(m => m.Id)
            .UseIdentityAlwaysColumn();

        modelBuilder.Entity<Model>()
            .HasMany<Vehicle>()
            .WithOne()
            .HasForeignKey(v => v.ModelId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Enterprise>()
            .ToTable("enterprise");

        modelBuilder.Entity<Enterprise>()
            .Property(e => e.Id)
            .UseIdentityAlwaysColumn();

        modelBuilder.Entity<Enterprise>()
            .HasMany<Vehicle>()
            .WithOne()
            .HasForeignKey(v => v.EnterpriseId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Enterprise>()
            .HasMany<Driver>()
            .WithOne()
            .HasForeignKey(d => d.EnterpriseId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Driver>()
            .ToTable("driver");

        modelBuilder.Entity<Driver>()
            .Property(d => d.Id)
            .UseIdentityAlwaysColumn();

        modelBuilder.Entity<DriverVehicleAssignment>()
            .ToTable("driver_vehicle_assignment")
            .HasKey(dva => new { dva.DriverId, dva.VehicleId });

        modelBuilder.Entity<DriverVehicleAssignment>()
            .HasOne<Driver>()
            .WithMany()
            .HasForeignKey(dva => dva.DriverId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DriverVehicleAssignment>()
            .HasOne<Vehicle>()
            .WithMany()
            .HasForeignKey(dva => dva.VehicleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DriverVehicleAssignment>()
            .HasIndex(dva => new { dva.DriverId, dva.IsActiveAssignment })
            .IsUnique()
            .HasFilter($"is_active_assignment = TRUE");

        modelBuilder.Entity<DriverVehicleAssignment>()
            .HasIndex(dva => new { dva.VehicleId, dva.IsActiveAssignment })
            .IsUnique()
            .HasFilter($"is_active_assignment = TRUE");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSeeding((context, _) =>
        {
            if (!CheckHasAnyData(context))
            {
                context.Set<Model>().AddRange(GetSeedModels());
                context.SaveChanges();

                context.Set<Enterprise>().AddRange(GetSeedEnterprises());
                context.SaveChanges();

                context.Set<Driver>().AddRange(GetSeedDrivers());
                context.SaveChanges();

                context.Set<Vehicle>().AddRange(GetSeedVehicles());
                context.SaveChanges();

                context.Set<DriverVehicleAssignment>().AddRange(GetSeedDriverVehicleAssignments());
                context.SaveChanges();
            }
        });

        optionsBuilder.UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
            if (!CheckHasAnyData(context))
            {
                await context.Set<Model>().AddRangeAsync(GetSeedModels(), cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                await context.Set<Enterprise>().AddRangeAsync(GetSeedEnterprises(), cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                await context.Set<Driver>().AddRangeAsync(GetSeedDrivers(), cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                await context.Set<Vehicle>().AddRangeAsync(GetSeedVehicles(), cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                await context.Set<DriverVehicleAssignment>().AddRangeAsync(GetSeedDriverVehicleAssignments(), cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }
        });
    }

    private bool CheckHasAnyData(DbContext context)
    {
        return context.Set<Model>().Any()
               || context.Set<Vehicle>().Any()
               || context.Set<Enterprise>().Any()
               || context.Set<Driver>().Any();
    }

    private IReadOnlyList<Model> GetSeedModels()
    {
        List<Model> models = new List<Model>
                {
                    // NoName модель
                    new Model()
                    {
                        ModelName = "NoName",
                        VehicleType = string.Empty,
                        SeatsCount = default,
                        MaxLoadingWeightKg = default,
                        EnginePowerKW = default,
                        TransmissionType = string.Empty,
                        FuelSystemType = string.Empty,
                        FuelTankVolumeLiters = string.Empty
                    },
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
                ModelId = 1, 
                EnterpriseId = 1,
                VinNumber = "VIN0001", 
                Price = 10000, 
                ManufactureYear = 2015, 
                Mileage = 50000,
                Color = "Красный"
            },
            new Vehicle
            {
                ModelId = 1, 
                EnterpriseId = 1,
                VinNumber = "VIN0002", 
                Price = 12000, 
                ManufactureYear = 2017, 
                Mileage = 30000,
                Color = "Синий"
            },
            // Грузовые автомобили
            new Vehicle
            {
                ModelId = 3, 
                EnterpriseId = 1,
                VinNumber = "VIN0005", 
                Price = 11000, 
                ManufactureYear = 2016, 
                Mileage = 45000,
                Color = "Зелёный"
            },
            new Vehicle
            {
                ModelId = 2, 
                EnterpriseId = 2,
                VinNumber = "VIN0003", 
                Price = 9000, 
                ManufactureYear = 2013, 
                Mileage = 70000,
                Color = "Чёрный"
            },
            new Vehicle
            {
                ModelId = 2, 
                EnterpriseId = 2,
                VinNumber = "VIN0004", 
                Price = 15000, 
                ManufactureYear = 2019, 
                Mileage = 20000,
                Color = "Белый"
            },
            new Vehicle
            {
                ModelId = 5, 
                EnterpriseId = 2,
                VinNumber = "VIN0009", 
                Price = 12500, 
                ManufactureYear = 2017, 
                Mileage = 40000,
                Color = "Коричневый"
            },
            // Мотоциклы
            new Vehicle
            {
                ModelId = 4, 
                EnterpriseId = 3,
                VinNumber = "VIN0007", 
                Price = 9500, 
                ManufactureYear = 2014, 
                Mileage = 60000,
                Color = "Серый"
            },
            new Vehicle
            {
                ModelId = 4, 
                EnterpriseId = 3,
                VinNumber = "VIN0008", 
                Price = 14000, 
                ManufactureYear = 2020, 
                Mileage = 15000,
                Color = "Бежевый"
            },
            new Vehicle
            {
                ModelId = 3, 
                EnterpriseId = 3,
                VinNumber = "VIN0006", 
                Price = 13000, 
                ManufactureYear = 2018, 
                Mileage = 35000,
                Color = "Серебристый"
            },
            // Тяжелые автобусы
            new Vehicle
            {
                ModelId = 6, 
                EnterpriseId = 3,
                VinNumber = "VIN0010", 
                Price = 16000, 
                ManufactureYear = 2021, 
                Mileage = 10000,
                Color = "Фиолетовый"
            }
        };

        return vehicles;
    }

    private IReadOnlyList<Enterprise> GetSeedEnterprises()
    {
        List<Enterprise> enterprises = new List<Enterprise>
        {
            new Enterprise
            {
                Name = "Извозкин.Такси-Парк",
                LegalAddress = "г. Москва, ул. Цифровая, д. 154"
            },
            new Enterprise
            {
                Name = "Delivery-Express",
                LegalAddress = "г. Санкт-Петербург, пр. Курьерский, д. 15"
            },
            new Enterprise
            {
                Name = "DoStavka",
                LegalAddress = "г. Москва, ул. Зеленая, д. 77"
            },
            new Enterprise
            {
                Name = "Red.Такси",
                LegalAddress = "г. Москва, ул. Красная, д. 21"
            }
        };

        return enterprises;
    }

    private IReadOnlyList<Driver> GetSeedDrivers()
    {
        List<Driver> drivers = new List<Driver>
        {
            // Водители для Извозкин.Такси-Парк (Enterprise Id = 1)
            new Driver
            {
                EnterpriseId = 1,
                FullName = "Иванов Иван Иванович",
                DriverLicenseNumber = "7777 123456"
            },
            new Driver
            {
                EnterpriseId = 1,
                FullName = "Петров Петр Петрович",
                DriverLicenseNumber = "7777 234567"
            },
            new Driver
            {
                EnterpriseId = 1,
                FullName = "Сидоров Сидор Сидорович",
                DriverLicenseNumber = "7777 345678"
            },
            
            // Водители для Delivery-Express (Enterprise Id = 2)
            new Driver
            {
                EnterpriseId = 2,
                FullName = "Александров Александр Александрович",
                DriverLicenseNumber = "7778 123456"
            },
            new Driver
            {
                EnterpriseId = 2,
                FullName = "Михайлов Михаил Михайлович",
                DriverLicenseNumber = "7778 234567"
            },
            new Driver
            {
                EnterpriseId = 2,
                FullName = "Николаев Николай Николаевич",
                DriverLicenseNumber = "7778 345678"
            },

            // Водители для DoStavka (Enterprise Id = 3)
            new Driver
            {
                EnterpriseId = 3,
                FullName = "Сергеев Сергей Сергеевич",
                DriverLicenseNumber = "7779 123456"
            },
            new Driver
            {
                EnterpriseId = 3,
                FullName = "Андреев Андрей Андреевич",
                DriverLicenseNumber = "7779 234567"
            }
        };

        return drivers;
    }

    private IReadOnlyList<DriverVehicleAssignment> GetSeedDriverVehicleAssignments()
    {
        List<DriverVehicleAssignment> assignments = new List<DriverVehicleAssignment>
        {
            // Извозкин.Такси-Парк (Enterprise Id = 1)
            // Иванов И.И. на красной Toyota Camry
            new DriverVehicleAssignment
            {
                DriverId = 1,
                VehicleId = 1,
                IsActiveAssignment = true  
            },
            new DriverVehicleAssignment
            {
                DriverId = 1,
                VehicleId = 2,
                IsActiveAssignment = false
            },
            new DriverVehicleAssignment
            {
                DriverId = 2,
                VehicleId = 1,
                IsActiveAssignment = false
            },
            new DriverVehicleAssignment
            {
                DriverId = 2,
                VehicleId = 2,
                IsActiveAssignment = false
            },
            // Минивэн остается без водителя (VehicleId = 5)

            // Delivery-Express (Enterprise Id = 2)
            // Александров А.А.
            new DriverVehicleAssignment
            {
                DriverId = 4,
                VehicleId = 3,
                IsActiveAssignment = true
            },
            new DriverVehicleAssignment
            {
                DriverId = 4,
                VehicleId = 4,
                IsActiveAssignment = false
            },
            // Михайлов М.М.
            new DriverVehicleAssignment
            {
                DriverId = 5,
                VehicleId = 3,
                IsActiveAssignment = false
            },
            new DriverVehicleAssignment
            {
                DriverId = 5,
                VehicleId = 4,
                IsActiveAssignment = false
            },
            // КАМАЗ остается без водителя (VehicleId = 9)

            // DoStavka (Enterprise Id = 3)
            // Сергеев С.С.
            new DriverVehicleAssignment
            {
                DriverId = 7,
                VehicleId = 7,
                IsActiveAssignment = true
            },
            new DriverVehicleAssignment
            {
                DriverId = 7,
                VehicleId = 8,
                IsActiveAssignment = false
            },
            // Андреев А.А.
            new DriverVehicleAssignment
            {
                DriverId = 8,
                VehicleId = 6,
                IsActiveAssignment = true
            },
            new DriverVehicleAssignment
            {
                DriverId = 8,
                VehicleId = 10,
                IsActiveAssignment = false
            }
        };

        return assignments;
    }
}