using CarPark.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles { get; set; } = default!;
    public DbSet<Model> Models { get; set; } = default!;
    public DbSet<Driver> Drivers { get; set; } = default!;
    public DbSet<Enterprise> Enterprises { get; set; } = default!;
    public DbSet<Manager> Managers { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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

        modelBuilder.Entity<Driver>()
            .HasMany(d => d.AssignedVehicles)
            .WithMany(v => v.AssignedDrivers)
            .UsingEntity("driver_vehicle_assignment");

        modelBuilder.Entity<Driver>()
            .HasOne(d => d.ActiveAssignedVehicle)
            .WithOne(v => v.ActiveAssignedDriver)
            .HasForeignKey<Driver>("assigned_vehicle_id")
            .IsRequired(false);

        modelBuilder.Entity<Manager>()
            .ToTable("manager");

        modelBuilder.Entity<Manager>()
            .Property(u => u.Id)
            .UseIdentityColumn();

        modelBuilder.Entity<Manager>()
            .HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(m => m.IdentityUserId)
            .IsRequired();

        modelBuilder.Entity<Manager>()
            .HasMany(m => m.Enterprises)
            .WithMany(e => e.Managers)
            .UsingEntity("enterprise_manager");
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

                context.Set<Vehicle>().AddRange(GetSeedVehicles());
                context.SaveChanges();

                context.Set<Driver>().AddRange(GetSeedDrivers(context));
                context.SaveChanges();

                context.Set<IdentityUser>().AddRange(SeedIdentityUsers());
                context.SaveChanges();

                context.Set<Manager>().AddRange(GetSeedManagers(context));
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

                await context.Set<Vehicle>().AddRangeAsync(GetSeedVehicles(), cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                await context.Set<Driver>().AddRangeAsync(GetSeedDrivers(context), cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                await context.Set<IdentityUser>().AddRangeAsync(SeedIdentityUsers(), cancellationToken);
                await context.SaveChangesAsync(cancellationToken);

                await context.Set<Manager>().AddRangeAsync(GetSeedManagers(context), cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
            }
        });
    }

    private bool CheckHasAnyData(DbContext context)
    {
        return context.Set<Model>().Any()
               || context.Set<Vehicle>().Any()
               || context.Set<Enterprise>().Any()
               || context.Set<Driver>().Any()
               || context.Set<IdentityUser>().Any()
               || context.Set<Manager>().Any();
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

    private IReadOnlyList<Driver> GetSeedDrivers(DbContext context)
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

        // Get existing vehicles from context to avoid tracking conflicts
        List<Vehicle> existingVehicles = context.Set<Vehicle>().ToList();

        drivers[0].AssignedVehicles = new List<Vehicle>
        {
            existingVehicles.First(v => v.Id == 1),
            existingVehicles.First(v => v.Id == 2),
        };
        drivers[0].ActiveAssignedVehicle = existingVehicles.First(v => v.Id == 1);

        drivers[1].AssignedVehicles = new List<Vehicle>
        {
            existingVehicles.First(v => v.Id == 1),
            existingVehicles.First(v => v.Id == 2),
        };

        drivers[3].AssignedVehicles = new List<Vehicle>
        {
            existingVehicles.First(v => v.Id == 4),
            existingVehicles.First(v => v.Id == 5),
        };
        drivers[3].ActiveAssignedVehicle = existingVehicles.First(v => v.Id == 4);

        drivers[4].AssignedVehicles = new List<Vehicle>
        {
            existingVehicles.First(v => v.Id == 4),
            existingVehicles.First(v => v.Id == 5),
        };

        drivers[6].AssignedVehicles = new List<Vehicle>
        {
            existingVehicles.First(v => v.Id == 7),
            existingVehicles.First(v => v.Id == 8),
        };
        drivers[6].ActiveAssignedVehicle = existingVehicles.First(v => v.Id == 7);

        drivers[7].AssignedVehicles = new List<Vehicle>
        {
            existingVehicles.First(v => v.Id == 9),
            existingVehicles.First(v => v.Id == 10),
        };
        drivers[7].ActiveAssignedVehicle = existingVehicles.First(v => v.Id == 9);

        return drivers;
    }

    private IReadOnlyList<IdentityUser> SeedIdentityUsers()
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

    private IReadOnlyList<Manager> GetSeedManagers(DbContext context)
    {
        List<Enterprise> existingEnterprises = context.Set<Enterprise>().ToList();
        
        List<Manager> managers = new List<Manager>
        {
            new Manager
            {
                IdentityUserId = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                Enterprises = existingEnterprises.Where(e => e.Id == 1 || e.Id == 2).ToList()
            },
            new Manager
            {
                IdentityUserId = "b2c3d4e5-f6g7-8901-bcde-f23456789012", 
                Enterprises = existingEnterprises.Where(e => e.Id == 2 || e.Id == 3).ToList()
            }
        };

        return managers;
    }
}